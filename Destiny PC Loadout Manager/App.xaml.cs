using Destiny_PC_Loadout_Manager.Auth;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Destiny_PC_Loadout_Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider provider;

        public App(): base()
        {
            var services = new ServiceCollection();

            var config = new Destiny2.Destiny2Config("DestinyPCLoadoutManager", "1.0", "30077", "", "");
            config.ApiKey = "c50bb382b1b84e1ba9640125c8f8f299";

            services.AddDestiny2(config);
            services.AddSingleton(new OAuthManager("30077", "55672"));
            services.AddSingleton(new AccountManager());

            provider = services.BuildServiceProvider();
        }
    }
}
