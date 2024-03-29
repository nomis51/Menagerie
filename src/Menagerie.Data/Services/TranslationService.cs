﻿using System.Diagnostics;
using System.Text.RegularExpressions;
using Menagerie.Data.Providers;
using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Models.Translation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Menagerie.Data.Services;

public class TranslationService : IService
{
    #region Constants

    private static readonly Dictionary<string, string> Languages = new()
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

    private readonly Dictionary<string, string> _mappedLanguages = new();
    private const string GoogleTranslateUrl = "https://translate.google.com";
    private const string GoogleTranslateBatchExecPath = "/_/TranslateWebserverUi/data/batchexecute?";

    #endregion

    #region Members

    private readonly HttpService _httpService;
    private readonly HttpService _urlEncodedHttpService;

    #endregion

    #region Constructors

    public TranslationService()
    {
        _httpService = new HttpService(new Uri(GoogleTranslateUrl));
        _urlEncodedHttpService = new HttpService(new Uri(GoogleTranslateUrl));
        _urlEncodedHttpService.Client.DefaultRequestHeaders.Remove("User-Agent");
        _urlEncodedHttpService.Client.DefaultRequestHeaders.Remove("X-Powered-By");
        _urlEncodedHttpService.Client.DefaultRequestHeaders.Remove("Accept");
        _urlEncodedHttpService.Client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");

        MapLanguages();
    }

    #endregion

    #region Public methods

    public void Initialize()
    {
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public IEnumerable<string> GetLanguages()
    {
        return Languages.Values.ToList();
    }

    public string LanguageToCode(string language)
    {
        return _mappedLanguages.ContainsKey(language) ? _mappedLanguages[language] : null;
    }

    public async Task<string?> Translate(string text, TranslationOptions options)
    {
        var gtSession = await GetSession(options).ConfigureAwait(false);

        if (gtSession == null)
        {
            Log.Warning("No session acquired for translation, aborting");
            return null;
        }

        var obfResponse = await GetBatchExecuteResponse(text, options, gtSession).ConfigureAwait(false);

        if (obfResponse == null)
        {
            Log.Warning("No translation response.");
            return null;
        }

        var (translatedMessage, sourceLanguage, destinationLanguage) = ReadGoogleTranslateResponse(obfResponse);

        if (!string.IsNullOrEmpty(translatedMessage)) return translatedMessage;

        Log.Warning("Unable to read translation response: {response}", obfResponse);
        return null;
    }

    #endregion

    #region Private methods

    private static Tuple<string, string, string> ReadGoogleTranslateResponse(string obfResponse)
    {
        try
        {
            // Yeah... we love obfuscated responses...
            obfResponse = obfResponse[obfResponse.IndexOf("[[", StringComparison.Ordinal)..];

            var endOfFirstBlock = obfResponse.IndexOf(",null,null,null,\"generic\"]", StringComparison.Ordinal);

            if (endOfFirstBlock == -1)
            {
                return new Tuple<string, string, string>("", "", "");
            }

            var firstBlock = obfResponse.Substring(1, endOfFirstBlock + ",null,null,null,\"generic\"]".Length);

            if (string.IsNullOrEmpty(firstBlock))
            {
                return new Tuple<string, string, string>("", "", "");
            }

            firstBlock = firstBlock.Replace("\\n", "");
            firstBlock = firstBlock[..^1];

            var parsedFirstBlock = JsonConvert.DeserializeObject<List<string>>(firstBlock);

            switch (parsedFirstBlock)
            {
                case {Count: < 3}:
                case null:
                    return new Tuple<string, string, string>("", "", "");
            }

            var secondBlock = parsedFirstBlock[2];
            secondBlock = secondBlock.Replace("\\n", "").Replace("\\", "");

            var parsedSecondBlock = JsonConvert.DeserializeObject<List<object>>(secondBlock);

            if (parsedSecondBlock == null) return new Tuple<string, string, string>("", "", "");
            if (parsedSecondBlock is {Count: < 2})
            {
                return new Tuple<string, string, string>("", "", "");
            }

            var thirdBlock = parsedSecondBlock[1];
            var textLang = parsedSecondBlock.Count >= 3 && parsedSecondBlock[2] != null
                ? (string) (parsedSecondBlock[2])
                : "";

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

            return new Tuple<string, string, string>("", "", "");
        }
        catch (Exception e)
        {
            Log.Warning("Unable to de-obfuscate Google Translate response: {response}. {message}. {stacktrace}",
                obfResponse, e.Message, e.StackTrace);
            Debugger.Break();
        }

        return new Tuple<string, string, string>("", "", "");
    }

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

    private static string GenerateBatchExecuteBody(string text, TranslationOptions options)
    {
        return "[[[\"MkEWBc\",\"[[" + $"\\\"{text}\\\"," + $"\\\"{options.FromLanguage}\\\"," +
               $"\\\"{options.ToLanguage}\\\"," +
               "true],[null]]\",null,\"generic\"]]]";
    }

    private async Task<GoogleTranslateSession> GetSession(TranslationOptions options)
    {
        var gotFromOption = !string.IsNullOrEmpty(options.FromLanguage);
        var gotToOption = !string.IsNullOrEmpty(options.ToLanguage);

        if (gotFromOption && !IsSupported(options.FromLanguage))
        {
            return null;
        }

        if (gotToOption && !IsSupported(options.ToLanguage))
        {
            return null;
        }

        if (!gotFromOption)
        {
            options.FromLanguage = "auto";
        }

        if (!gotToOption)
        {
            options.ToLanguage = "en";
        }

        try
        {
            var response = await _httpService.Client.GetAsync("/").ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                Log.Error("Unable to create session: " + "HTTP " + response.StatusCode +
                          response.Content.ReadAsStringAsync().Result);
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
                ReqId = (int)Math.Floor(1000.0d + ((new Random()).NextDouble() * 9000.0d)),
                RT = "c"
            };
        }
        catch (Exception e)
        {
            Log.Error("Unable to call google translation session");
            return null;
        }
    }

    private async Task<string> GetBatchExecuteResponse(string text, TranslationOptions options,
        GoogleTranslateSession gtSession)
    {
        var url = $"{GoogleTranslateBatchExecPath}{gtSession.ToQueryString()}";
        var body = GenerateBatchExecuteBody(text, options);

        var values = new Dictionary<string, string> {{"f.req", body}};

        var batchResult = await _urlEncodedHttpService.Client.PostAsync(url, new FormUrlEncodedContent(values)).ConfigureAwait(false);

        if (!batchResult.IsSuccessStatusCode)
        {
            Log.Error("Unable to translate text: " + "HTTP " + batchResult.StatusCode +
                      batchResult.Content.ReadAsStringAsync().Result);
            return null;
        }

        return await batchResult.Content.ReadAsStringAsync();
    }

    #endregion
}