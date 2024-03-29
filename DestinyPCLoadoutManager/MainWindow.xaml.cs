﻿using DestinyPCLoadoutManager.API;
using DestinyPCLoadoutManager.Auth;
using DestinyPCLoadoutManager.Controls;
using DestinyPCLoadoutManager.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DestinyPCLoadoutManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var oauthManager = App.provider.GetService(typeof(OAuthManager)) as OAuthManager;
            oauthManager.AuthEvent += AuthChanged;

            // Main loadouts
            var loadouts = Properties.Settings.Default.Loadouts;

            if (loadouts != null)
            {
                var saverList = new List<LoadoutSaver> { loadout0, loadout1, loadout2, loadout3, loadout4 };

                var index = 0;
                foreach (var pair in loadouts.Zip(saverList))
                {
                    pair.Second.SetLoadout(pair.First, index);
                    index += 1;
                }
            }

            var farmingLoadout = Properties.Settings.Default.FarmingLoadout;

            if (farmingLoadout != null)
            {
                loadoutFarming.SetLoadout(farmingLoadout, Properties.Settings.Default.IsFarmingMode);
            }

            // TODO: Move
            Logic.InputManager inputManager = App.provider.GetService(typeof(Logic.InputManager)) as Logic.InputManager;
            inputManager.RegisterShortcut($"fullscreenTest", new API.Models.Shortcut(Key.P, ModifierKeys.Control | ModifierKeys.Shift), OpenWindow);
        }

        public void AuthenticateClick(object sender, RoutedEventArgs e)
        {
            var oauthManager = App.provider.GetService(typeof(OAuthManager)) as OAuthManager;
            var url = oauthManager.GenerateAuthorizationUrl();
            oauthManager.VisitUrl(url);
        }

        public async void FetchUserClick(object sender, RoutedEventArgs e)
        {
            var oauthManager = App.provider.GetService(typeof(OAuthManager)) as OAuthManager;
            var accountManager = App.provider.GetService(typeof(AccountManager)) as AccountManager;

            if (!oauthManager.IsAuthorized)
            {
                return;
            }

            var characters = await accountManager.GetCharacters();

            if (characters.Count() < 1)
            {
                // No characters
                return;
            }

            characterList.SetCharacters(characters);
        }

        private void AuthChanged(object sender, bool e)
        {
            authLabel.Content = e ? "Authorized" : "Not Authorized";

            if (e)
            {
                FetchUserClick(null, null);
            }
        }

        private void OpenWindow()
        {
            var window = new ItemSearchWindow();
            window.Show();
        }
    }
}
