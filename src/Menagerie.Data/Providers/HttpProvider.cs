using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Menagerie.Data.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Menagerie.Data.Providers;

public static class HttpProvider
{
    #region Members

    public static readonly HttpService PoeNinja = new(new Uri("https://poe.ninja"));
    public static readonly HttpService AnonymousPoeApi = new(new Uri("http://api.pathofexile.com"));
    private static HttpService? _poeWebsite;
    private static HttpService? _poeApi;

    #endregion

    #region Props

    public static HttpService? PoeApi
    {
        get
        {
            if (_poeApi is not null) return _poeApi;
            var settings = AppDataService.Instance.GetSettings();

            _poeApi = new HttpService(new Uri("http://api.pathofexile.com"), new List<Cookie>
            {
                new("POESESSID", settings.General.Poesessid)
            });

            return _poeApi;
        }
    }

    public static HttpService? PoeWebsite
    {
        get
        {
            if (_poeWebsite is not null) return _poeWebsite;
            var settings = AppDataService.Instance.GetSettings();

            _poeWebsite = new HttpService(new Uri("https://www.pathofexile.com"), new List<Cookie>
            {
                new("POESESSID", settings.General.Poesessid)
            });

            return _poeWebsite;
        }
    }

    #endregion

    #region Public methods

    public static async Task<T?> ReadResponse<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonConvert.DeserializeObject<T>(content, new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
            }
            catch (JsonSerializationException e)
            {
                Log.Error("Error while deserializing response", e);
            }
            catch (Exception e)
            {
                Log.Error("Error while reading response", e);
            }
        }
        else
        {
            var message = response.Content.ReadAsStringAsync().Result;
            Log.Error("Errored response content: {Message}", message);
        }

        return default;
    }

    public static HttpContent? SerializeBody<T>(T obj)
    {
        try
        {
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        catch (Exception e)
        {
            Log.Error("Serializing body content for {} {}", typeof(T), e.Message);
        }

        return null;
    }

    #endregion
}

public class HttpService
{
    #region Props

    public HttpClient Client { get; private set; }

    #endregion

    #region Constructors

    public HttpService(Uri baseUrl)
    {
        Client = new HttpClient(new HttpClientHandler { UseProxy = false, Proxy = null });
        SetupClient(baseUrl);
    }

    public HttpService(Uri baseUrl, List<Cookie> cookies)
    {
        Client = new HttpClient(GetHandler(baseUrl, cookies));
        SetupClient(baseUrl);
    }

    #endregion

    #region Private methods

    private static HttpClientHandler GetHandler(Uri baseUrl, List<Cookie> cookies)
    {
        var cookieContainer = new CookieContainer();

        cookies.ForEach(c => cookieContainer.Add(baseUrl, c));

        return new HttpClientHandler { CookieContainer = cookieContainer, UseProxy = false, Proxy = null };
    }

    private void SetupClient(Uri baseUrl)
    {
        Client.BaseAddress = baseUrl;
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Menagerie");
        Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Menagerie");
    }

    #endregion
}