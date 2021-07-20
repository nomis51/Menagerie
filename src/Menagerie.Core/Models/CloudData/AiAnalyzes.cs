using System.Collections;
using System.Collections.Generic;
using Menagerie.Core.Models.ML;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Menagerie.Core.Models.CloudData
{
    public class AiAnalyzes
    {
        [JsonProperty("ai_predictions")]
        public PredictionResponse AiPredictions { get; set; }
        public List<PredictionImage> Images { get; set; } = new();
    }
}