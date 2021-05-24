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
using Newtonsoft.Json.Linq;

namespace Menagerie.Core.Services
{
    public class TranslateService : IService
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(TranslateService));

        private static readonly Dictionary<string, string> Languages = new Dictionary<string, string>()
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

        private readonly Dictionary<string, string> _mappedLanguages = new Dictionary<string, string>();
        private const string GoogleTranslateUrl = "https://translate.google.com";
        private const string GoogleTranslateBatchExecPath = "/_/TranslateWebserverUi/data/batchexecute?";

        #endregion

        #region Members

        private readonly HttpService _httpService;
        private readonly HttpService _urlEncodedHttpService;

        #endregion

        #region Constructors

        public TranslateService()
        {
            Log.Trace("Initializing TranslateService");

            _httpService = new HttpService(new Uri(GoogleTranslateUrl));
            _urlEncodedHttpService = new HttpService(new Uri(GoogleTranslateUrl));
            _urlEncodedHttpService.Client.DefaultRequestHeaders.Remove("User-Agent");
            _urlEncodedHttpService.Client.DefaultRequestHeaders.Remove("X-Powered-By");
            _urlEncodedHttpService.Client.DefaultRequestHeaders.Remove("Accept");
            _urlEncodedHttpService.Client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");

            MapLanguages();
        }

        #endregion

        private void MapLanguages()
        {
            foreach (var (key, value) in Languages.Where(l => !_mappedLanguages.ContainsKey(l.Value)))
            {
                _mappedLanguages.Add(value, key);
            }
        }

        private string GetCode(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                return null;
            }

            if (Languages.ContainsKey(language))
            {
                return language;
            }

            return _mappedLanguages.ContainsKey(language) ? _mappedLanguages[language] : null;
        }

        private bool IsSupported(string language)
        {
            return GetCode(language) != null;
        }

        private static string Extract(string key, string result)
        {
            var matches = Regex.Match(result, $"\"{key}\":\".*?\"");

            if (!matches.Success) return "";
            var temp = matches.Value.Replace($"\"{key}\":\"", "");
            return temp[..^1];
        }

        private static string GenerateBatchExecuteBody(string text, TranslateOptions options)
        {
            return ($"[[[\"MkEWBc\",\"[[" + $"\\\"{text}\\\"," + $"\\\"{options.From}\\\"," + $"\\\"{options.To}\\\"," +
                    "true],[null]]\",null,\"generic\"]]]");
        }

        private async Task<GoogleTranslateSession> GetSession(TranslateOptions options)
        {
            var gotFromOption = !string.IsNullOrEmpty(options.From);
            var gotToOption = !string.IsNullOrEmpty(options.To);

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
            var url = $"{GoogleTranslateBatchExecPath}{gtSession.ToQueryString()}";
            var body = GenerateBatchExecuteBody(text, options);

            var values = new Dictionary<string, string> {{"f.req", body}};

            var batchResult = await _urlEncodedHttpService.Client.PostAsync(url, new FormUrlEncodedContent(values));

            if (!batchResult.IsSuccessStatusCode)
            {
                return null;
            }

            return await batchResult.Content.ReadAsStringAsync();
        }

        public static IEnumerable<string> GetLanguages()
        {
            return Languages.Values.Skip(1).ToList();
        }

        private static Tuple<string, string, string> ReadGoogleTranslateResponse(string obfResponse)
        {
            // Yeah... we love obfuscated responses...
            obfResponse = obfResponse[obfResponse.IndexOf("[[", StringComparison.Ordinal)..];

            var endOfFirstBlock = obfResponse.IndexOf(",null,null,null,\"generic\"]", StringComparison.Ordinal);

            if (endOfFirstBlock == -1)
            {
                return null;
            }

            var firstBlock = obfResponse.Substring(1, endOfFirstBlock + ",null,null,null,\"generic\"]".Length);

            if (string.IsNullOrEmpty(firstBlock))
            {
                return null;
            }

            firstBlock = firstBlock.Replace("\\n", "");

            var parsedFirstBlock = JsonConvert.DeserializeObject<List<string>>(firstBlock);

            switch (parsedFirstBlock)
            {
                case {Count: < 3}:
                case null:
                    return null;
            }

            var secondBlock = parsedFirstBlock[2];
            secondBlock = secondBlock.Replace("\\n", "").Replace("\\", "");

            var parsedSecondBlock = JsonConvert.DeserializeObject<List<object>>(secondBlock);

            if (parsedSecondBlock == null) return null;
            if (parsedSecondBlock is {Count: < 2})
            {
                return null;
            }

            var thirdBlock = parsedSecondBlock[1];
            var textLang = parsedSecondBlock.Count >= 3 && parsedSecondBlock[2] != null ? (string) (parsedSecondBlock[2]) : "";

            try
            {
                var array = (JArray) ((JArray) thirdBlock)[0][0]?[5];
                var translatedText = "";

                if (array != null)
                    foreach (var element in array)
                    {
                        try
                        {
                            translatedText += (string) (element[0]) + " ";
                        }
                        catch (Exception)
                        {
                            translatedText += (string) element + " ";
                        }
                    }

                var translationLang = (string) ((JArray) thirdBlock)[1];

                if (translationLang != null)
                    return new Tuple<string, string, string>(translatedText,
                        Languages.ContainsKey(textLang) ? Languages[textLang] : textLang.ToUpper(),
                        Languages.ContainsKey(translationLang)
                            ? Languages[translationLang]
                            : translationLang.ToUpper());
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return null;
        }

        public string LanguageToCode(string lang)
        {
            return _mappedLanguages.ContainsKey(lang) ? _mappedLanguages[lang] : null;
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

            var (item1, item2, item3) = ReadGoogleTranslateResponse(obfResponse);
            translation.TranslatedMessage = item1;
            translation.OriginalLang = string.IsNullOrEmpty(item2) ? Languages[options.From] : item2;
            translation.TranslationLang = item3;

            AppService.Instance.TextTranslated(translation);
        }

        public void Start()
        {
        }
    }
}