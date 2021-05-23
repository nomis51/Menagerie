﻿using log4net;
using Menagerie.ViewModels;
using System.Windows;
using Menagerie.Core.Extensions;
using AdonisUI.Controls;
using System.Collections.Generic;
using System.Windows.Controls;
using Menagerie.Core.Services;

namespace Menagerie.Views
{
    /// <summary>
    /// Logique d'interaction pour ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : AdonisWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConfigWindow));

        private readonly ConfigViewModel _vm;

        private readonly List<Grid> _views = new();

        public ConfigWindow()
        {
            InitializeComponent();

            Log.Trace("Initializing ConfigWindow");

            _vm = new ConfigViewModel();
            DataContext = _vm;

            SetupViews();
            ShowView("grdGeneral");
        }

        private void SetupViews()
        {
            _views.Add(grdApi);
            _views.Add(grdGeneral);
            _views.Add(grdWhispers);
            _views.Add(grdChatScan);
            _views.Add(grdChaosRecipe);
        }

        private void ShowView(string name)
        {
            foreach (var v in _views)
            {
                v.Visibility = v.Name == name ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("Cancel button clicked");
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("Save button clicked");
            _vm.SaveConfig();
            Close();
        }

        private void itGeneral_Click(object sender, RoutedEventArgs e)
        {
            ShowView("grdGeneral");
        }

        private void itApi_Click(object sender, RoutedEventArgs e)
        {
            ShowView("grdApi");
        }

        private void itWhispers_Click(object sender, RoutedEventArgs e)
        {
            ShowView("grdWhispers");
        }

        private void itChatScan_Click(object sender, RoutedEventArgs e)
        {
            ShowView("grdChatScan");
        }

        private void itChaosRecipe_Click(object sender, RoutedEventArgs e)
        {
            ShowView("grdChaosRecipe");
        }

        private void btnResetDefaultOverlay_Click(object sender, RoutedEventArgs e)
        {
            _vm.Config.ChaosRecipeOveralyDockMode = true;
            _vm.Config.ChaosRecipeGridOffset = new System.Drawing.Point(0, 0);
            _vm.Config.OutgoingOffersGridOffset = new System.Drawing.Point(0, 0);
            _vm.Config.IncomingOffersControlsGridOffset = new System.Drawing.Point(0, 0);
            _vm.Config.IncomingOffersGridOffset = new System.Drawing.Point(0, 0);
            _vm.SaveConfig();

            AppService.Instance.ResetDefaultOverlay();
        }
    }
}