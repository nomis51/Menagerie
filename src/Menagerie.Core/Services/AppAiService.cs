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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Models.ML;
using Menagerie.Core.Models.Parsing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NumSharp;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using Tensorflow.NumPy;
using Buffer = System.Buffer;
using NDArray = Tensorflow.NumPy.NDArray;
using np = Tensorflow.NumPy.np;
using Shape = Tensorflow.Shape;

namespace Menagerie.Core.Services
{
    public class AppAiService : IService
    {
        #region Constant

        private const string PYTHON_392_ZIP_URL =
            "https://www.python.org/ftp/python/3.9.2/python-3.9.2-embed-amd64.zip";

        private const string PYTHON_FOLDER = "./ML/python/";
        private const string LOCAL_PYTHON_EXE_PATH = "./ML/python/python.exe";
        private const string ML_SERVER_PATH = "./server.py";
        private const string TRAINED_MODELS_FOLDER = "./ML/trained/";

        private const string TRAINED_MODELS_LATEST_RELEASE_URL =
            "https://github.com/nomis51/poe-ml/releases/download/v1.0.0/{}.zip";

        private const string TRAINED_MODELS_VERSION_URL =
            "https://github.com/nomis51/poe-ml/releases/download/v1.0.0/version.txt";

        public const string TEMP_FOLDER = "./ML/.temp/";
        private readonly Uri _pythonServerUrl = new("http://localhost:8302");
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private bool _pythonServerReady;

        private readonly List<string> _trainedModelsName = new()
        {
            "currency_type",
            "stack_size",
        };

        #endregion

        #region Members

        private readonly bool _useLocalPython = false;
        private ProcessStartInfo _pythonServerProcessInfos;
        private Process _pythonServerProcess;
        private readonly HttpService _httpService;
        private readonly Dictionary<string, Functional> _models = new();
        private readonly Dictionary<string, List<string>> _labels = new();
        private readonly Dictionary<string, Size> _inputSizes = new();

        #endregion

        #region Constructors

        public AppAiService()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            _httpService = new HttpService(_pythonServerUrl);
        }

        #endregion

        #region Private methods

        private void InitializeCurrencyTypeNeuralNetwork()
        {
            const string MODEL_NAME = "currency_type";

            var modelInputSize = new Size(46, 46);

            _labels.Add(MODEL_NAME,
                File.ReadAllText("./ML/trained/currency_type/currency_type-classes.txt").Split(",").ToList());
            var nbClasses = _labels[MODEL_NAME].Count-1;
            _inputSizes.Add(MODEL_NAME, modelInputSize);

            var layers = new LayersApi();
            var inputs =
                keras.Input(shape: (_inputSizes[MODEL_NAME].Height, _inputSizes[MODEL_NAME].Width, 3));

            var x = layers.Conv2D(16, (3, 3), activation: "relu").Apply(inputs);
            x = layers.MaxPooling2D(2, 2).Apply(x);

            x = layers.Conv2D(32, (3, 3), activation: "relu").Apply(x);
            x = layers.MaxPooling2D(2, 2).Apply(x);

            x = layers.Conv2D(64, (3, 3), activation: "relu").Apply(x);
            x = layers.MaxPooling2D(2, 2).Apply(x);

            x = layers.Flatten().Apply(x);

            x = layers.Dense(512, activation: "relu").Apply(x);
            var outputs = layers.Dense(nbClasses, activation: "softmax").Apply(x);

            _models[MODEL_NAME] = keras.Model(inputs, outputs);
            _models[MODEL_NAME].compile(keras.optimizers.RMSprop(0.001f), keras.losses.CategoricalCrossentropy(),
                new[] {"acc"});
            _models[MODEL_NAME].load_weights($"./ML/trained/{MODEL_NAME}/{MODEL_NAME}.h5");
        }

        private void InitializeStackSizeNeuralNetwork()
        {

            keras.backend.clear_session();
            const string MODEL_NAME = "stack_size";
            var modelInputSize = new Size(46, 46);

            _labels.Add(MODEL_NAME,
                File.ReadAllText($"./ML/trained/{MODEL_NAME}/{MODEL_NAME}-classes.txt").Split(",").ToList());
            var nbClasses = _labels[MODEL_NAME].Count-1;
            _inputSizes.Add(MODEL_NAME, modelInputSize);

            var layers = new LayersApi();
            var inputs =
                keras.Input(shape: (_inputSizes[MODEL_NAME].Height, _inputSizes[MODEL_NAME].Width, 3));

            var x = layers.Conv2D(16, (3, 3), activation: "relu").Apply(inputs);
            x = layers.MaxPooling2D(2, 2).Apply(x);

            x = layers.Conv2D(32, (3, 3), activation: "relu").Apply(x);
            x = layers.MaxPooling2D(2, 2).Apply(x);

            x = layers.Conv2D(64, (3, 3), activation: "relu").Apply(x);
            x = layers.MaxPooling2D(2, 2).Apply(x);

            x = layers.Flatten().Apply(x);

            x = layers.Dense(512, activation: "relu").Apply(x);
            var outputs = layers.Dense(nbClasses, activation: "softmax").Apply(x);

            _models[MODEL_NAME] = keras.Model(inputs, outputs);
            _models[MODEL_NAME].compile(keras.optimizers.RMSprop(0.001f), keras.losses.CategoricalCrossentropy(),
                new[] {"acc"});
            _models[MODEL_NAME].load_weights($"./ML/trained/{MODEL_NAME}/{MODEL_NAME}.h5");
        }

        private void InitializeNeuralNetwork()
        {
            tf.enable_eager_execution();
            InitializeCurrencyTypeNeuralNetwork();
           // InitializeStackSizeNeuralNetwork();
        }

        private List<string> Predict(string filePath, List<string> modelNames)
        {
            List<string> results = new();

            foreach (var modelName in modelNames)
            {
                // TODO: deal with empty tiles

                var nd = ReadTensorFromImageFile(filePath, _inputSizes[modelName].Height, _inputSizes[modelName].Width);

                var predictions = _models[modelName].predict(nd);

                var score = tf.nn.softmax(predictions[0]);
                var scoreArray = score.numpy();
                var index = np.argmax(scoreArray[0]);
                results.Add(_labels[modelName][index]);
            }

            return results;
        }

        public Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            var result = new Bitmap(width, height);
            using var g = Graphics.FromImage(result);
            g.DrawImage(bmp, 0, 0, width, height);
            return result;
        }


        private NDArray ReadTensorFromImageFile(string filePath, int height = 224, int width = 224)
        {
            var graph = tf.get_default_graph();
            graph.building_function = true;

            var fileReader = tf.io.read_file(filePath, "file_reader");
            var decodeJpeg = tf.image.decode_jpeg(fileReader, channels: 3, name: "DecodeJpeg");
            var cast = tf.cast(decodeJpeg, tf.float32);
            var dimsExpander = tf.expand_dims(cast, 0);
            var resize = tf.constant(new int[] {height, width});
            var bilinear = tf.image.resize_bilinear(dimsExpander, resize);

            using var sess = tf.Session(graph);
            return sess.run(bilinear);
        }

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
            AppService.ShowDebugMessage("Starting Ai server...");

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


                    _ = Task.Run(() =>
                    {
                        Thread.Sleep(10000);
                        AppService.ShowDebugMessage("");
                    });

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
            AppService.ShowDebugMessage("Verifying AI models...");

            if (_trainedModelsName.Any(trainedModelName =>
                !File.Exists($"{TRAINED_MODELS_FOLDER}/{trainedModelName}/{trainedModelName}.h5") ||
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

        private void DownloadTrainedModels()
        {
            AppService.ShowDebugMessage("Downloading AI models...");

            EnsureTrainedFolderExists();
            using var client = new WebClient();

            foreach (var name in _trainedModelsName)
            {
                var zipFilePath = $"{TRAINED_MODELS_FOLDER}/{name}.zip";
                client.DownloadFile(TRAINED_MODELS_LATEST_RELEASE_URL.Replace("{}", name), zipFilePath);

                ZipFile.ExtractToDirectory(zipFilePath, $"{TRAINED_MODELS_FOLDER}/");

                File.Delete(zipFilePath);
            }

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
            AppService.ShowDebugMessage("Checking dependencies...");

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

        private static void EnsureDependenciesInstalled()
        {
            var start = new ProcessStartInfo
            {
                FileName = "python.exe",
                Arguments = "-m ensurepip",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var pipCheckProcess = Process.Start(start);

            if (pipCheckProcess == null) throw new ApplicationException("Cannot verify pip installation");

            pipCheckProcess.WaitForExit();
            pipCheckProcess.Close();

            start = new ProcessStartInfo
            {
                FileName = "python.exe",
                Arguments = "-m pip install --user -r ./ML/requirements.txt",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var depsInstallProcess = Process.Start(start);

            if (depsInstallProcess == null) throw new ApplicationException("Cannot install ML server dependencies");

            var errors = depsInstallProcess.StandardError.ReadToEnd();

            depsInstallProcess.WaitForExit();
            depsInstallProcess.Close();
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
                    graphics.DrawImage(image, new Rectangle(0, 0, width, height),
                        new Rectangle(i * width, j * height, width - (offseter == 0 ? 0 : 1), height),
                        GraphicsUnit.Pixel);
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
                var bounds = new Rectangle(528, 63, 201, 85);
                var cvRefMat = CvInvoke.Imread("./Assets/trade-window-title.png", Emgu.CV.CvEnum.ImreadModes.Grayscale);
                var cvRefImage = cvRefMat.ToImage<Gray, byte>();
                const int delay = 1000;

                while (true)
                {
                    timer.Restart();
                    var image = AppService.Instance.CaptureArea(bounds);
                    image.Save("test.png");
                    var cvImage = image.ToImage<Gray, byte>();

                    var result = cvImage.MatchTemplate(cvRefImage, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);

                    double[] minValues, maxValues;
                    Point[] minLocations, maxLocations;
                    result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                    result.Dispose();

                    if (maxValues[0] > 0.9)
                    {
                        _ = Task.Run(() => AppService.Instance.AnalyzeTradeWindow());
                    }

                    timer.Stop();

                    var sleepTime = delay - (int) timer.ElapsedMilliseconds;

                    Thread.Sleep(sleepTime < 0 ? 0 : sleepTime);
                }
                // ReSharper disable once FunctionNeverReturns
            });
        }

        #endregion

        #region Public methods

        public void ClosePythonServer()
        {
            _pythonServerProcess.Close();
        }

        public List<string> ProcessAndSaveTradeWindowImage(Bitmap image)
        {
            var images = SliceImage(image, AppService.Instance.TradeWindowRowsAndColumns.Width,
                AppService.Instance.TradeWindowRowsAndColumns.Height,
                AppService.Instance.TradeWindowSquareSize.Width, AppService.Instance.TradeWindowSquareSize.Height);
            List<string> paths = new();

            if (!Directory.Exists(TEMP_FOLDER))
            {
                Directory.CreateDirectory(TEMP_FOLDER);
            }

            images.ForEach(i =>
            {
                var id = Guid.NewGuid().ToString();
                var path = $"{TEMP_FOLDER}{id}.jpeg";
                i.Save(path, ImageFormat.Jpeg);
                paths.Add(path);
            });

            return paths;
        }

        public List<Price> PredictTradeWindowCurrencies(IEnumerable<string> filePaths)
        {
            var results = Predict(filePaths, new List<string> {"currency_type", "stack_size"});

            List<Price> prices = new();

            foreach (var result in results)
            {
                try
                {
                    prices.Add(new Price
                    {
                        Currency = result[0],
                        Value = float.Parse(result[1])
                    });
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return prices;
        }

        private List<List<string>> Predict(IEnumerable<string> filePaths, List<string> modelNames)
        {
            return filePaths.Select(path => Predict(path, modelNames)).ToList();
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
            // Task.Run(() =>
            // {
            //     if (!IsPythonInstalled())
            //     {
            //         return;
            //         // TODO: notify python is not installed
            //     }
            //
            //     EnsureDependenciesInstalled();
            //
            //     _pythonServerProcessInfos = new ProcessStartInfo()
            //     {
            //         FileName = _useLocalPython ? LOCAL_PYTHON_EXE_PATH : "python.exe",
            //         Arguments = ML_SERVER_PATH,
            //         WorkingDirectory = Environment.CurrentDirectory + "/ML/",
            //         UseShellExecute = false,
            //         RedirectStandardError = true,
            //         RedirectStandardOutput = true,
            //         CreateNoWindow = true
            //     };
            //
            //     EnsureTrainedModelsAvailable();
            //
            //     StartPythonServer();
            //     KeepPythonServerAlive();
            //     CleanTempFolder();
            //     LookForTradeWindow();
            // });

            //EnsureTrainedModelsAvailable();
            InitializeNeuralNetwork();
            LookForTradeWindow();
        }

        #endregion
    }
}