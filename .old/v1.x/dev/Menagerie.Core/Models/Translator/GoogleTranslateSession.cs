using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace Menagerie.Core.Models.Translator
{
    public class GoogleTranslateSession
    {
        [JsonProperty("rpcids")] public string RpcIds { get; set; }

        [JsonProperty("f.sid")] public string FSid { get; set; }

        [JsonProperty("bl")] public string BL { get; set; }

        [JsonProperty("hl")] public string HL { get; set; }

        [JsonProperty("soc-platform")] public int SocPlatform { get; set; }

        [JsonProperty("soc-device")] public int SocDevice { get; set; }

        [JsonProperty("soc-app")] public int SocApp { get; set; }

        [JsonProperty("_reqid")] public int ReqId { get; set; }

        [JsonProperty("rt")] public string RT { get; set; }

        public string ToQueryString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"rpcids={HttpUtility.UrlEncode(RpcIds)}&");
            sb.Append($"f.sid={HttpUtility.UrlEncode(FSid)}&");
            sb.Append($"bl={HttpUtility.UrlEncode(BL)}&");
            sb.Append($"hl={HttpUtility.UrlEncode(HL)}&");
            sb.Append($"soc-app={HttpUtility.UrlEncode(SocApp.ToString())}&");
            sb.Append($"soc-platform={HttpUtility.UrlEncode(SocPlatform.ToString())}&");
            sb.Append($"soc-device={HttpUtility.UrlEncode(SocDevice.ToString())}&");
            sb.Append($"_reqid={HttpUtility.UrlEncode(ReqId.ToString())}&");
            sb.Append($"rt={HttpUtility.UrlEncode(RT)}");

            return sb.ToString();
        }
    }
}