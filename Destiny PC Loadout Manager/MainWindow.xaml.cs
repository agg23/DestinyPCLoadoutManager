using DestinyPCLoadoutManager.Auth;
using Destiny2;
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
        }

        private async void FetchUserClick(object sender, RoutedEventArgs e)
        {
            var destinyApi = App.provider.GetService(typeof(IDestiny2)) as IDestiny2;
            var oauthManager = App.provider.GetService(typeof(OAuthManager)) as OAuthManager;
            var accountManager = App.provider.GetService(typeof(AccountManager)) as AccountManager;

            if (!oauthManager.IsAuthorized)
            {
                return;
            }

            var account = await accountManager.GetAccount();
            var profile = await destinyApi.GetProfile(oauthManager.currentToken.access_token, (BungieMembershipType) 3, account.MembershipId);

            System.Diagnostics.Debug.WriteLine(profile);
        }
    }
}
