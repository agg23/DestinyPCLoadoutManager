using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DestinyPCLoadoutManager.Auth
{
    /// <summary>
    /// Interaction logic for OAuthControl.xaml
    /// </summary>
    public partial class OAuthControl : UserControl
    {
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
    }
}
