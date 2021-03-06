﻿using Menagerie.Core.Services;
using Menagerie.Core.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Menagerie.Models;
using Serilog;
using CoreModels = Menagerie.Core.Models;

namespace Menagerie.ViewModels
{
    public class ConfigViewModel : Screen
    {
        #region Props

        public Config Config { get; set; }
        public BindableCollection<string> Leagues { get; set; }

        public string ChatScanWords
        {
            get => string.Join(" ", Config.ChatScanWords);
            set { Config.ChatScanWords = value.Split(' ').Select(w => w.ToLower()).ToList(); }
        }

        #endregion


        public ConfigViewModel()
        {
            Log.Information("Intializing ConfigViewModel");

            Task.Run(() =>
            {
                Config = AppMapper.Instance.Map<CoreModels.Config, Config>(AppService.Instance.GetConfig());

                Leagues = new();
                Leagues.AddRange(AppService.Instance.GetLeagues().Result);

                NotifyOfPropertyChange(() => Leagues);
            });
        }

        public void SaveConfig()
        {
            Log.Information("Saving config");
            AppService.Instance.SetConfig(AppMapper.Instance.Map<Config, CoreModels.Config>(Config));
        }
    }
}