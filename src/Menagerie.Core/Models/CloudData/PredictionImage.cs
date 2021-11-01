using Newtonsoft.Json;

namespace Menagerie.Core.Models.CloudData
{
    public class PredictionImage
    {
        [JsonProperty("base64_image")]
        public string Base64Image { get; set; }
        
        [JsonProperty("file_id")]
        public string FileId { get; set; }
    }
}