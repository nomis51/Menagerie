using System.Collections.Generic;
using Newtonsoft.Json;

namespace Menagerie.Core.Models.ML
{
    public class PredictionResponseImage
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        
        [JsonProperty("file_id")]
        public string FileId { get; set; }
        
        [JsonProperty("file_path")]
        public string FilePath { get; set; }
        
        public List<PredictionResponseValue> Predictions { get; set; }
    }
}