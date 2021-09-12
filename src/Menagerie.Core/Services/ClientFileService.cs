using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Menagerie.Core.Extensions;
using log4net;
using Menagerie.Core.Models.Parsing.Abstractions;
using Menagerie.Core.Models.Parsing.Entries;
using Menagerie.Core.Models.Trades;
using Menagerie.Core.Services.Parsing;
using Newtonsoft.Json;

namespace Menagerie.Core.Services
{
    public class ClientFileService : IService
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(ClientFileService));
        private const string LocationsFile = @".\Data\locations.json";

        #endregion

        #region Members

        private readonly Dictionary<string, Area> _areas = new Dictionary<string, Area>();

        #endregion

        #region Props

        public LogService LogService { get; private set; }

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
            LogService = new LogService(logFilePath);
            LogService.NewTradeLogEntry += LogServiceOnNewTradeLogEntry;
            LogService.NewAreaChangeLogEntry += LogServiceOnNewAreaChangeLogEntry;
            LogService.NewPlayerJoinedAreaLogEntry += LogServiceOnNewPlayerJoinedAreaLogEntry;
            LogService.NewChatMessageLogEntry += LogServiceOnNewChatMessageLogEntry;
            LogService.NewLogEntry += LogServiceOnNewLogEntry;
            LogService.NewTradeStateLogEntry += LogServiceOnNewTradeStateLogEntry;
        }

        private static void LogServiceOnNewTradeStateLogEntry(TradeStateLogEntry logEntry)
        {
            if (logEntry.IsAccepted)
            {
                AppService.Instance.OnTradeAccepted();
            }
            else if (logEntry.IsCancelled)
            {
                AppService.Instance.OnTradeCancelled();
            }
        }

        private static void LogServiceOnNewChatMessageLogEntry(ChatMessageLogEntry logEntry)
        {
            AppService.Instance.ChatScan(logEntry);
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

        private static void LogServiceOnNewLogEntry(ILogEntry logEntry)
        {
            var g = 0;
            // TODO: deal with generic logEntries
        }

        private static void LogServiceOnNewPlayerJoinedAreaLogEntry(PlayerJoinedAreaLogEntry logEntry)
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