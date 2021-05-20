using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menagerie.Core.Extensions;
using log4net;
using Newtonsoft.Json;
using PoeLogsParser.Models;
using PoeLogsParser.Models.Abstractions;
using PoeLogsParser.Services;

namespace Menagerie.Core.Services
{
    public class ClientFileService : IService
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientFileService));
        private const string LocationsFile = @".\Data\locations.json";

        #endregion

        #region Members

        private LogService _logService;
        private readonly Dictionary<string, Area> _areas = new Dictionary<string, Area>();

        #endregion

        #region Props

        public LogService LogService => _logService;

        #endregion

        #region Constructors

        public ClientFileService()
        {
            Log.Trace("Initializing ClientFileService");
        }

        #endregion

        #region Private methods

        public void StartWatching(string logFilePath)
        {
            _logService = new LogService(logFilePath);
            _logService.NewTradeLogEntry += LogServiceOnNewTradeLogEntry;
            _logService.NewAreaChangeLogEntry += LogServiceOnNewAreaChangeLogEntry;
            _logService.NewPlayerJoinedAreaLogEntry += LogServiceOnNewPlayerJoinedAreaLogEntry;
            _logService.NewLogEntry += LogServiceOnNewLogEntry;
        }

        private void LoadLocations()
        {
            Log.Trace("Loading locations");

            try
            {
                var str = File.ReadAllText(LocationsFile);
                var areas = JsonConvert.DeserializeObject<List<Area>>(str);

                if (areas == null) return;
                foreach (var area in areas.Where(area => !_areas.ContainsKey(area.Name)))
                {
                    _areas.Add(area.Name, area);
                }
            }
            catch (Exception e)
            {
                Log.Error("Error while reading locations", e);
            }
        }

        private void LogServiceOnNewLogEntry(ILogEntry logEntry)
        {
            // TODO: deal with generic logEntries
        }

        private void LogServiceOnNewPlayerJoinedAreaLogEntry(PlayerJoinedAreaLogEntry logEntry)
        {
            AppService.Instance.NewPlayerJoined(logEntry.Player);
        }

        private void LogServiceOnNewAreaChangeLogEntry(AreaChangeLogEntry logEntry)
        {
            if (!logEntry.Area.ToLower().Contains("hideout"))
            {
                AppService.Instance.StashApiUpdated();
            }

            var type = _areas.ContainsKey(logEntry.Area) ? _areas[logEntry.Area].Type : "";

            AppService.Instance.SetCurrentArea(logEntry.Area, type);
        }

        private void LogServiceOnNewTradeLogEntry(TradeLogEntry logEntry)
        {
            AppService.Instance.NewOffer(new Offer(logEntry));
        }

        #endregion

        #region Public methods

        public void Start()
        {
            Log.Trace("Starting ClientFileService");
            LoadLocations();
        }

        #endregion
    }
}