using Menagerie.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Menagerie.Core.Extensions;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Menagerie.Models;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;
using CoreModels = Menagerie.Core.Models;

namespace Menagerie.ViewModels
{
    public class ConfigViewModel : Screen
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConfigViewModel));

        #region Props

        public ReactiveProperty<Config> Config { get; set; }
        public ReactiveProperty<BindableCollection<string>> Leagues { get; set; }

        public string ChatScanWords
        {
            get => string.Join(" ", Config.Value.ChatScanWords);
            set { Config.Value.ChatScanWords = value.Split(' ').Select(w => w.ToLower()).ToList(); }
        }

        #endregion


        public ConfigViewModel()
        {
            Log.Trace("Intializing ConfigViewModel");

            Config = new ReactiveProperty<Config>("Config", this, AppMapper.Instance.Map<CoreModels.Config, Config>(AppService.Instance.GetConfig()));

            BindableCollection<string> leagues = new();
            leagues.AddRange(AppService.Instance.GetLeagues().Result);
            Leagues = new ReactiveProperty<BindableCollection<string>>("Leagues", this, leagues);
        }

        public void SaveConfig()
        {
            Log.Trace("Saving config");
            AppService.Instance.SetConfig(AppMapper.Instance.Map<Config, CoreModels.Config>(Config.Value));
        }
    }
}