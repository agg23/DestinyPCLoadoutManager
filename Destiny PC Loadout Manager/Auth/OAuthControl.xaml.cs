// Copyright 2016 Google Inc.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;

namespace Destiny_PC_Loadout_Manager.Auth
{
    /// <summary>
    /// Interaction logic for OAuthControl.xaml
    /// </summary>
    public partial class OAuthControl : UserControl {
        OAuthManager oauthManager = App.provider.GetService(typeof(OAuthManager)) as OAuthManager;

        public OAuthControl()
        {
            InitializeComponent();
        }

        private void AuthenticateClick(object sender, RoutedEventArgs e)
        {
            var url = oauthManager.GenerateAuthorizationUrl();
            oauthManager.VisitUrl(url);
        }

        private async void EnterCodeClick(object sender, RoutedEventArgs e)
        {
            await oauthManager.ExchangeCode(txtBox.Text);
        }
    }
}
