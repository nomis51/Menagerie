using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menagerie.Core.Models.ML
{
    public class PredictionResponse
    {
        public bool Error { get; set; }
        public string Message { get; set; }

        [JsonProperty("elapsed_time")]
        public long ElapsedTime { get; set; }
        public IEnumerable<Prediction> Predictions { get; set; }
    }
}
