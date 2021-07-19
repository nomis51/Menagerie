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
        [JsonProperty("elapsed_time")]
        public long ElapsedTime { get; set; }
        
        public IEnumerable<PredictionResponseImage> Images { get; set; }
    }
}
