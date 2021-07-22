using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using LiteDB.Engine;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Enums;
using Menagerie.Core.Models;
using Menagerie.Core.Models.ML;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Menagerie.Core.Services
{
    public class AppAiService : IService
    {
        #region Constant

        private const string PYTHON_392_ZIP_URL = "https://www.python.org/ftp/python/3.9.2/python-3.9.2-embed-amd64.zip";
        private const string PYTHON_FOLDER = "./ML/python/";
        private const string LOCAL_PYTHON_EXE_PATH = "./ML/python/python.exe";
        private const string ML_SERVER_PATH = "./server.py";
        private const string TRAINED_MODELS_FOLDER = "./ML/trained/";
        private const string TRAINED_MODELS_LATEST_RELEASE_URL = "https://github.com/nomis51/poe-ml/releases/download/v1.0.0/trained_models.zip";
        private const string TRAINED_MODELS_VERSION_URL = "https://github.com/nomis51/poe-ml/releases/download/v1.0.0/version.txt";
        public const string TEMP_FOLDER = "./ML/.temp/";
        private readonly Uri _pythonServerUrl = new("http://localhost:8302");
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private bool _pythonServerReady;

        private readonly List<string> _trainedModelsName = new()
        {
            "currency_type",
            "stack_size",
            "item_links",
            "item_sockets",
            "socket_color"
        };

        #endregion

        #region Members

        private readonly bool _useLocalPython = false;
        private readonly ProcessStartInfo _pythonServerProcessInfos;
        private Process _pythonServerProcess;
        private readonly HttpService _httpService;

        #endregion

        #region Constructors

        public AppAiService()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            _httpService = new HttpService(_pythonServerUrl);

            if (!IsPythonInstalled())
            {
                DownloadPython();
                _useLocalPython = true;
            }

            _pythonServerProcessInfos = new ProcessStartInfo()
            {
                FileName = _useLocalPython ? LOCAL_PYTHON_EXE_PATH : "python.exe",
                Arguments = ML_SERVER_PATH,
                WorkingDirectory = Environment.CurrentDirectory + "/ML/",
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            EnsureTrainedModelsAvailable();
        }

        #endregion

        #region Private methods

        private void CleanTempFolder()
        {
            if (Directory.Exists(TEMP_FOLDER))
            {
                Directory.Delete(TEMP_FOLDER, true);
            }

            Directory.CreateDirectory(TEMP_FOLDER);
        }

        private void StartPythonServer()
        {
            Task.Run(async () =>
            {
                try
                {
                    var verifyResponse = await _httpService.Client.GetAsync("/verify");

                    if (verifyResponse.IsSuccessStatusCode)
                    {
                        var response = await HttpService.ReadResponse<Dictionary<string, bool>>(verifyResponse);

                        if (response != null && response["ok"])
                        {
                            var reloadResponse = await _httpService.Client.GetAsync("/reload");

                            if (reloadResponse.IsSuccessStatusCode) return;

                            throw new Exception("ML server unavailable");
                        }
                    }
                    else if (verifyResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        // TODO: something is already running on the current port, so change ML server port
                        throw new NotImplementedException("Change ML server port");
                    }
                }
                catch
                {
                    // ignored
                }

                while (true)
                {
                    _pythonServerProcess = Process.Start(_pythonServerProcessInfos);

                    if (_pythonServerProcess != null)
                    {
                        // TODO: log to serilog
                        // var errorOutput = _pythonServerProcess.StandardError.ReadToEnd();
                        // var output = _pythonServerProcess.StandardOutput.ReadToEnd();
                        _pythonServerReady = true;
                        _pythonServerProcess.WaitForExit();
                        _pythonServerProcess.Close();
                    }

                    Thread.Sleep(5000);
                }
            });
            // ReSharper disable once FunctionNeverReturns
        }

        private void EnsureTrainedModelsAvailable()
        {
            if (_trainedModelsName.Any(trainedModelName => !File.Exists($"{TRAINED_MODELS_FOLDER}/{trainedModelName}/{trainedModelName}.h5") ||
                                                           !File.Exists($"{TRAINED_MODELS_FOLDER}/{trainedModelName}/{trainedModelName}-classes.txt")))
            {
                if (Directory.Exists(TRAINED_MODELS_FOLDER))
                {
                    Directory.Delete(TRAINED_MODELS_FOLDER, true);
                }

                DownloadTrainedModels();
            }
            else
            {
                var versionPath = $"{TRAINED_MODELS_FOLDER}/version.txt";
                var tempVersionPath = $"{TRAINED_MODELS_FOLDER}/temp-version.txt";

                if (!File.Exists(versionPath)) return;

                var currentVersion = File.ReadAllText(versionPath);

                using var client = new WebClient();
                client.DownloadFile(TRAINED_MODELS_VERSION_URL, tempVersionPath);

                var tempVersion = File.ReadAllText(tempVersionPath);

                if (tempVersion != currentVersion)
                {
                    if (Directory.Exists(TRAINED_MODELS_FOLDER))
                    {
                        Directory.Delete(TRAINED_MODELS_FOLDER, true);
                    }

                    DownloadTrainedModels();
                }

                if (File.Exists(tempVersionPath))
                {
                    File.Delete(tempVersionPath);
                }
            }
        }

        private static void DownloadTrainedModels()
        {
            EnsureTrainedFolderExists();

            using var client = new WebClient();
            var zipFilePath = $"{TRAINED_MODELS_FOLDER}/trainedModels.zip";
            client.DownloadFile(TRAINED_MODELS_LATEST_RELEASE_URL, zipFilePath);

            ZipFile.ExtractToDirectory(zipFilePath, $"{TRAINED_MODELS_FOLDER}/");

            File.Delete(zipFilePath);

            client.DownloadFile(TRAINED_MODELS_VERSION_URL, $"{TRAINED_MODELS_FOLDER}/version.txt");
        }

        private static void EnsureTrainedFolderExists()
        {
            if (!Directory.Exists(TRAINED_MODELS_FOLDER))
            {
                Directory.CreateDirectory(TRAINED_MODELS_FOLDER);
            }
        }

        private static void EnsurePythonFolderExists()
        {
            if (!Directory.Exists(PYTHON_FOLDER))
            {
                Directory.CreateDirectory(PYTHON_FOLDER);
            }
        }

        private static void DownloadPython()
        {
            EnsurePythonFolderExists();

            using var client = new WebClient();
            var zipFilePath = $"{PYTHON_FOLDER}/python392.zip";
            client.DownloadFile(PYTHON_392_ZIP_URL, zipFilePath);

            ZipFile.ExtractToDirectory(zipFilePath, $"{PYTHON_FOLDER}/");
        }

        private static bool IsPythonInstalled()
        {
            var start = new ProcessStartInfo
            {
                FileName = "python.exe",
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(start);

            if (process == null) throw new ApplicationException("Cannot verify python installation.");

            using var reader = process.StandardError;
            var result = reader.ReadToEnd();

            return string.IsNullOrEmpty(result);
        }

        private List<Bitmap> SliceImage(Image image, int rows, int cols, int width, int height)
        {
            List<Bitmap> images = new(rows * cols);
            int offseter = 0;

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    var bmp = new Bitmap(width, height);
                    var graphics = Graphics.FromImage(bmp);
                    graphics.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(i * width, j * height, width - (offseter == 0 ? 0 : 1), height), GraphicsUnit.Pixel);
                    graphics.Dispose();
                    images.Add(bmp);

                    // Dumb code, but the trade window doesn't keep the same square size everywhere
                    // Looks like 53x53 in Gimp, but if you check the whole window size
                    // 12x53 != trade window width and 5x53 != trade window height
                    // There's a ~0.4 offset on the width and ~0.2 offset on the height
                    ++offseter;
                    if (offseter > 3)
                    {
                        offseter = 0;
                    }
                }
            }

            return images;
        }

        private static void RemoveImages(PredictionRequest request)
        {
            foreach (var image in request.Images)
            {
                var path = image.FilePath.Replace("./", "./ML/");

                if (!File.Exists(path)) continue;

                File.Delete(path);
            }
        }

        private void KeepPythonServerAlive()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var response = await _httpService.Client.GetAsync("/verify");

                    Thread.Sleep(5000);
                }
            });
        }

        private void LookForTradeWindow()
        {
            Task.Run(() =>
            {
                Stopwatch timer = new();
                var bounds = new Rectangle(528, 63, 201, 61);
                var cvRefMat = CvInvoke.Imread("./Assets/trade-window-title.png", Emgu.CV.CvEnum.ImreadModes.Grayscale);
                var cvRefImage = cvRefMat.ToImage<Gray, byte>();
                int sleepTime = 1000;

                while (true)
                {
                    timer.Restart();
                    var image = AppService.Instance.CaptureArea(bounds);
                    var cvImage = image.ToImage<Gray, byte>();

                    var result = cvRefImage.MatchTemplate(cvRefImage, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);

                    double[] minValues, maxValues;
                    Point[] minLocations, maxLocations;
                    result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                    if (maxValues[0] > 0.9)
                    {
                        _ = Task.Run(async () => await AppService.Instance.AnalyzeTradeWindow());
                    }

                    timer.Stop();

                    sleepTime = 1000 - (int)timer.ElapsedMilliseconds;

                    Thread.Sleep(sleepTime < 0 ? 0 : sleepTime);
                }
            });
        }

        #endregion

        #region Public methods

        public void ClosePythonServer()
        {
            _pythonServerProcess.Close();
        }

        public List<Tuple<string, string>> ProcessAndSaveTradeWindowImage(Bitmap image)
        {
            var images = SliceImage(image, AppService.Instance.TradeWindowRowsAndColumns.Width, AppService.Instance.TradeWindowRowsAndColumns.Height,
                AppService.Instance.TradeWindowSquareSize.Width, AppService.Instance.TradeWindowSquareSize.Height);
            List<Tuple<string, string>> imageIds = new();

            if (!Directory.Exists(TEMP_FOLDER))
            {
                Directory.CreateDirectory(TEMP_FOLDER);
            }

            images.ForEach(i =>
            {
                var id = Guid.NewGuid().ToString();
                var path = $"{TEMP_FOLDER}{id}.jpeg";
                i.Save(path, ImageFormat.Jpeg);
                imageIds.Add(new Tuple<string, string>(id, path.Replace("ML/", "")));
            });

            return imageIds;
        }

        public async Task<PredictionResponse> Predict(PredictionRequest request)
        {
            if (!_pythonServerReady) return new PredictionResponse();

            try
            {
                var body = new StringContent(JsonConvert.SerializeObject(request, _jsonSerializerSettings), Encoding.UTF8, "application/json");
                var response = await _httpService.Client.PostAsync($"/api", body);

                if (!response.IsSuccessStatusCode) return default;

                return await HttpService.ReadResponse<PredictionResponse>(response);
            }
            catch (Exception e)
            {
                var g = 0;
            }

            return new PredictionResponse();
        }

        // public async Task<PredictionResponseImage> Predict(TrainedModelType trainedModelType, string imageFileId)
        // {
        //     if (string.IsNullOrEmpty(imageFileId) || !File.Exists($"{TEMP_FOLDER}{imageFileId}.jpeg")) return default;
        //
        //     var trainingName = TrainedModelTypeConverter.Convert(trainedModelType);
        //
        //     if (string.IsNullOrEmpty(trainingName)) return default;
        //
        //     var response = await _httpService.Client.GetAsync($"{trainingName}?file_id={imageFileId}");
        //
        //     if (!response.IsSuccessStatusCode) return default;
        //
        //     return await HttpService.ReadResponse<PredictionResponseImage>(response);
        // }

        public void Start()
        {
            StartPythonServer();
            KeepPythonServerAlive();
            CleanTempFolder();
            LookForTradeWindow();
        }

        #endregion
    }
}