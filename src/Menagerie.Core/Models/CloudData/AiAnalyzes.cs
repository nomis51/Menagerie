using System.Collections;
using System.Collections.Generic;
using Menagerie.Core.Models.ML;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Menagerie.Core.Models.CloudData
{
    public class AiAnalyzes
    {
        //[BsonId]
        //[JsonProperty("_id")]
        //public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [JsonProperty("ai_predictions")]
        public PredictionResponse AiPredictions { get; set; }

        public List<PredictionImage> Images { get; set; } = new();
    }
}