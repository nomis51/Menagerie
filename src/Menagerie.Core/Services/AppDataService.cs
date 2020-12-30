using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Menagerie.Core.Models;
using Menagerie.Core.Abstractions;
using System.Threading.Tasks;

namespace Menagerie.Core.Services {
    public class AppDataService : IService {
        #region Constants
        private const string DATA_FOLDER = @".\Data\";
        private const string STATS = "stats.json";
        private const string BASE_TYPES = "base-types.json";
        #endregion

        private Dictionary<string, MatchStr> _statByMatchStr;
        private List<Tuple<string, BaseType>> _baseTypes;

        public AppDataService() { }

        public Dictionary<string, MatchStr> GetStatByMatchStr() {
            if (_statByMatchStr != null && _statByMatchStr.Count > 0) {
                return _statByMatchStr;
            }

            var data = File.ReadAllText(Path.Combine(DATA_FOLDER, STATS));
            var dtos = JsonConvert.DeserializeObject<List<StatDto>>(data);

            Dictionary<string, MatchStr> stats = new Dictionary<string, MatchStr>();

            foreach (var dto in dtos) {
                foreach (var condition in dto.Conditions) {
                    stats.Add(condition.String, new MatchStr() {
                        Matcher = new StatMatcher(condition),
                        Stat = new Stat(dto.Mod),
                        Matchers = dto.Conditions.Select(c => new StatMatcher(c)).ToList()
                    });
                }
            }

            _statByMatchStr = stats;

            return _statByMatchStr;
        }

        public List<Tuple<string, BaseType>> GetBaseTypes() {
            if (_baseTypes != null && _baseTypes.Count > 0) {
                return _baseTypes;
            }

            var data = File.ReadAllText(Path.Combine(DATA_FOLDER, BASE_TYPES));
            var objs = JsonConvert.DeserializeObject<List<List<object>>>(data);

            var parsedObjs = objs.Select(o => {
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(o[1]));
                return new Tuple<string, Dictionary<string, string>>((string)o[0], parsed);
            });

            var baseTypes = parsedObjs.Select(e => new Tuple<string, BaseType>(e.Item1, new BaseType() {
                Category = Item.ToItemCategory(e.Item2["category"]),
                Icon = e.Item2.ContainsKey("icon") ? e.Item2["icon"] : null
            })).ToList();

            _baseTypes = baseTypes;

            return _baseTypes;
        }

        public void Start() {
            Task.Run(() => _ = GetStatByMatchStr());
            Task.Run(() => _ = GetBaseTypes());
        }
    }

    public class StatDto {
        public List<StatConditionDto> Conditions { get; set; } = new List<StatConditionDto>();
        public StatModDto Mod { get; set; }
    }

    public class StatModDto {
        public string Text { get; set; }
        public string Ref { get; set; }
        public bool Inverted { get; set; }
        public List<StatModTypeDto> Types { get; set; }
    }

    public class StatModTypeDto {
        public string Name { get; set; }
        public List<string> TradeId { get; set; } = new List<string>();
    }

    public class StatConditionDto {
        public string String { get; set; }
        public string Ref { get; set; }
        public bool Negate { get; set; }
        public StatConditionConditionDto Condition { get; set; }
        public StatConditionOptionDto Option { get; set; }
    }

    public class StatConditionOptionDto {
        public string Text { get; set; }
        public string Ref { get; set; }
        public int TradeId { get; set; }
    }

    public class StatConditionConditionDto {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class MatchStr {
        public StatMatcher Matcher { get; set; }
        public Stat Stat { get; set; }
        public List<StatMatcher> Matchers { get; set; } = new List<StatMatcher>();
    }
}
