using Newtonsoft.Json;

namespace Menagerie.Core.Models.ML
{
    public class Prediction
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        [JsonProperty("file_id")]
        public string FileId { get; set; }
        public string Value { get; set; }
    }
}