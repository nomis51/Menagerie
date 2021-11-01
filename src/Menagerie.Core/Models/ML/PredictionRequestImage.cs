using System.Collections.Generic;
using Newtonsoft.Json;

namespace Menagerie.Core.Models.ML
{
    public class PredictionRequestImage
    {
        [JsonProperty("file_id")]
        public string FileId { get; set; }
        
        [JsonProperty("file_path")]
        public string FilePath { get; set; }
        
        public List<string> Models { get; set; }
    }
}