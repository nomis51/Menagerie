using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Extensions;
using Menagerie.Core.Models.Translator;
using Newtonsoft.Json;

namespace Menagerie.Core.Services
{
    public class TranslateService : IService
    {
        #region Constants

        private static readonly ILog log = LogManager.GetLogger(typeof(TranslateService));

        private static readonly Dictionary<string, string> LANGAGES = new Dictionary<string, string>()
        {
            {"auto", "Automatic"},
            {"en", "English"},
            {"ru", "Russian"},
            {"de", "German"},
            {"ko", "Korean"},
            {"es", "Spanish"},
            {"fr", "French"},
            {"af", "Afrikaans"},
            {"sq", "Albanian"},
            {"am", "Amharic"},
            {"ar", "Arabic"},
            {"hy", "Armenian"},
            {"az", "Azerbaijani"},
            {"eu", "Basque"},
            {"be", "Belarusian"},
            {"bn", "Bengali"},
            {"bs", "Bosnian"},
            {"bg", "Bulgarian"},
            {"ca", "Catalan"},
            {"ceb", "Cebuano"},
            {"ny", "Chichewa"},
            {"zh-CN", "Chinese {Simplified}"},
            {"zh-TW", "Chinese {Traditional}"},
            {"co", "Corsican"},
            {"hr", "Croatian"},
            {"cs", "Czech"},
            {"da", "Danish"},
            {"nl", "Dutch"},
            {"eo", "Esperanto"},
            {"et", "Estonian"},
            {"tl", "Filipino"},
            {"fi", "Finnish"},
            {"fy", "Frisian"},
            {"gl", "Galician"},
            {"ka", "Georgian"},
            {"el", "Greek"},
            {"gu", "Gujarati"},
            {"ht", "Haitian Creole"},
            {"ha", "Hausa"},
            {"haw", "Hawaiian"},
            {"he", "Hebrew"},
            {"iw", "Hebrew"},
            {"hi", "Hindi"},
            {"hmn", "Hmong"},
            {"hu", "Hungarian"},
            {"is", "Icelandic"},
            {"ig", "Igbo"},
            {"id", "Indonesian"},
            {"ga", "Irish"},
            {"it", "Italian"},
            {"ja", "Japanese"},
            {"jw", "Javanese"},
            {"kn", "Kannada"},
            {"kk", "Kazakh"},
            {"km", "Khmer"},
            {"ku", "Kurdish {Kurmanji}"},
            {"ky", "Kyrgyz"},
            {"lo", "Lao"},
            {"la", "Latin"},
            {"lv", "Latvian"},
            {"lt", "Lithuanian"},
            {"lb", "Luxembourgish"},
            {"mk", "Macedonian"},
            {"mg", "Malagasy"},
            {"ms", "Malay"},
            {"ml", "Malayalam"},
            {"mt", "Maltese"},
            {"mi", "Maori"},
            {"mr", "Marathi"},
            {"mn", "Mongolian"},
            {"my", "Myanmar {Burmese}"},
            {"ne", "Nepali"},
            {"no", "Norwegian"},
            {"ps", "Pashto"},
            {"fa", "Persian"},
            {"pl", "Polish"},
            {"pt", "Portuguese"},
            {"pa", "Punjabi"},
            {"ro", "Romanian"},
            {"sm", "Samoan"},
            {"gd", "Scots Gaelic"},
            {"sr", "Serbian"},
            {"st", "Sesotho"},
            {"sn", "Shona"},
            {"sd", "Sindhi"},
            {"si", "Sinhala"},
            {"sk", "Slovak"},
            {"sl", "Slovenian"},
            {"so", "Somali"},
            {"su", "Sundanese"},
            {"sw", "Swahili"},
            {"sv", "Swedish"},
            {"tg", "Tajik"},
            {"ta", "Tamil"},
            {"te", "Telugu"},
            {"th", "Thai"},
            {"tr", "Turkish"},
            {"uk", "Ukrainian"},
            {"ur", "Urdu"},
            {"uz", "Uzbek"},
            {"vi", "Vietnamese"},
            {"cy", "Welsh"},
            {"xh", "Xhosa"},
            {"yi", "Yiddish"},
            {"yo", "Yoruba"},
            {"zu", "Zulu"}
        };

        private Dictionary<string, string> MAPPED_LANGAGES = new Dictionary<string, string>();
        private const string GOOGLE_TRANSLATE_URL = "https://translate.google.com";
        private const string GOOGLE_TRANSLATE_BATCH_EXEC_PATH = "/_/TranslateWebserverUi/data/batchexecute?";

        #endregion

        #region Members

        private HttpService _httpService;
        private HttpService _urlEncodedHttpService;

        #endregion

        #region Constructors

        public TranslateService()
        {
            log.Trace("Initializing TranslateService");

            _httpService = new HttpService(new Uri(GOOGLE_TRANSLATE_URL));
            _urlEncodedHttpService = new HttpService(new Uri(GOOGLE_TRANSLATE_URL));
            _urlEncodedHttpService.Client.DefaultRequestHeaders.Remove("User-Agent");
            _urlEncodedHttpService.Client.DefaultRequestHeaders.Remove("X-Powered-By");
            _urlEncodedHttpService.Client.DefaultRequestHeaders.Remove("Accept");
            _urlEncodedHttpService.Client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");

            MapLangages();
        }

        #endregion

        private void MapLangages()
        {
            foreach (var l in LANGAGES)
            {
                if (!MAPPED_LANGAGES.ContainsKey(l.Value))
                {
                    MAPPED_LANGAGES.Add(l.Value, l.Key);
                }
            }
        }

        private string GetCode(string langage)
        {
            if (string.IsNullOrEmpty(langage))
            {
                return null;
            }

            if (LANGAGES.ContainsKey(langage))
            {
                return langage;
            }

            if (MAPPED_LANGAGES.ContainsKey(langage))
            {
                return MAPPED_LANGAGES[langage];
            }

            return null;
        }

        private bool IsSupported(string langage)
        {
            return GetCode(langage) != null;
        }

        private string Extract(string key, string result)
        {
            var matches = Regex.Match(result, $"\"{key}\":\".*?\"");

            if (matches.Success)
            {
                var temp = matches.Value.Replace($"\"{key}\":\"", "");
                return temp.Substring(0, temp.Length - 1);
            }

            return "";
        }

        private string GenerateBatchExecuteBody(string text, TranslateOptions options)
        {
            return ($"[[[\"MkEWBc\",\"[[" + $"\\\"{text}\\\"," + $"\\\"{options.From}\\\"," + $"\\\"{options.To}\\\"," +
                    "true],[null]]\",null,\"generic\"]]]");
        }

        private async Task<GoogleTranslateSession> GetSession(TranslateOptions options)
        {
            bool gotFromOption = !string.IsNullOrEmpty(options.From);
            bool gotToOption = !string.IsNullOrEmpty(options.To);

            if (gotFromOption && !IsSupported(options.From))
            {
                return null;
            }

            if (gotToOption && !IsSupported(options.To))
            {
                return null;
            }

            if (!gotFromOption)
            {
                options.From = "auto";
            }

            if (!gotToOption)
            {
                options.To = "en";
            }

            var response = await _httpService.Client.GetAsync("/");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadAsStringAsync();

            return new GoogleTranslateSession()
            {
                RpcIds = "MkEWBc",
                FSid = Extract("FdrFJe", result),
                BL = Extract("cfb2h", result),
                HL = "en-US",
                SocApp = 1,
                SocDevice = 1,
                SocPlatform = 1,
                ReqId = (int) Math.Floor(1000.0d + ((new Random()).NextDouble() * 9000.0d)),
                RT = "c"
            };
        }

        private async Task<string> GetBatchExecuteResponse(string text, TranslateOptions options,
            GoogleTranslateSession gtSession)
        {
            string url = $"{GOOGLE_TRANSLATE_BATCH_EXEC_PATH}{gtSession.ToQueryString()}";
            var body = GenerateBatchExecuteBody(text, options);

            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("f.req", body);

            var batchResult = await _urlEncodedHttpService.Client.PostAsync(url, new FormUrlEncodedContent(values));

            if (!batchResult.IsSuccessStatusCode)
            {
                return null;
            }

            return await batchResult.Content.ReadAsStringAsync();
        }

        private Tuple<string, string, string> ReadGoogleTranslateResponse(string obfResponse)
        {
            // Yeah... we love obfuscated responses...
            obfResponse = obfResponse.Substring(obfResponse.IndexOf("[["));

            int endOfFirstBlock = obfResponse.IndexOf(",null,null,null,\"generic\"]");

            if (endOfFirstBlock == -1)
            {
                return null;
            }

            string firstBlock = obfResponse.Substring(1, endOfFirstBlock + ",null,null,null,\"generic\"]".Length);

            if (string.IsNullOrEmpty(firstBlock))
            {
                return null;
            }

            firstBlock = firstBlock.Replace("\\n", "");

            var parsedFirstBlock = JsonConvert.DeserializeObject<List<string>>(firstBlock);

            if (parsedFirstBlock.Count < 3)
            {
                return null;
            }

            var secondBlock = parsedFirstBlock[2];
            secondBlock = secondBlock.Replace("\\n", "").Replace("\\", "");

            var parsedSecondBlock = JsonConvert.DeserializeObject<List<object>>(secondBlock);

            if (parsedSecondBlock.Count < 2)
            {
                return null;
            }

            var thirdBlock = parsedSecondBlock[1];
            string textLang = (string) (parsedSecondBlock[2]);

            try
            {
                var array = (Newtonsoft.Json.Linq.JArray) ((Newtonsoft.Json.Linq.JArray) thirdBlock)[0][0][5];
                string translatedText = "";

                for (int i = 0; i < array.Count; ++i)
                {
                    var element = array[i];

                    try
                    {
                        translatedText += (string) (element[0]) + " ";
                    }
                    catch (Exception)
                    {
                        translatedText += (string) element + " ";
                    }
                }

                string translationLang = (string) ((Newtonsoft.Json.Linq.JArray) thirdBlock)[1];

                return new Tuple<string, string, string>(translatedText,
                    LANGAGES.ContainsKey(textLang) ? LANGAGES[textLang] : textLang.ToUpper(),
                    LANGAGES.ContainsKey(translationLang) ? LANGAGES[translationLang] : translationLang.ToUpper());
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return null;
        }

        public List<string> GetLangages()
        {
            return LANGAGES.Values.Skip(1).ToList();
        }

        public string LangageToCode(string lang)
        {
            return MAPPED_LANGAGES.ContainsKey(lang) ? MAPPED_LANGAGES[lang] : null;
        }

        public string CodeToLangage(string code)
        {
            return LANGAGES.ContainsKey(code) ? LANGAGES[code] : null;
        }

        public async Task Translate(ChatMessageTranslation translation, TranslateOptions options)
        {
            var gtSession = await GetSession(options);

            if (gtSession == null)
            {
                return;
            }

            var obfResponse = await GetBatchExecuteResponse(translation.OriginalMessage, options, gtSession);

            if (obfResponse == null)
            {
                return;
            }

            var result = ReadGoogleTranslateResponse(obfResponse);
            translation.TranslatedMessage = result.Item1;
            translation.OriginalLang = result.Item2;
            translation.TranslationLang = result.Item3;

            AppService.Instance.TextTranslated(translation);
        }

        public void Start()
        {
        }
    }
}