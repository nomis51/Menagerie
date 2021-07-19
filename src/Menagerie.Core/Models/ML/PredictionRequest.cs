using System.Collections.Generic;

namespace Menagerie.Core.Models.ML
{
    public class PredictionRequest
    {
        public List<PredictionRequestImage> Images { get; set; } = new();
    }
}