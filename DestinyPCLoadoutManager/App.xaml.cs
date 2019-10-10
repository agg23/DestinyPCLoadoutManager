using DestinyPCLoadoutManager.API;
using DestinyPCLoadoutManager.Auth;
using Gear.NamedPipesSingleInstance;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DestinyPCLoadoutManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string APP_NAME = "DestinyPCLoadoutManager";
        public static ServiceProvider provider;

        private static async Task OnUiThreadAsync(Action action)
        {
            if (Current.Dispatcher.CheckAccess())
            {
                action();
                return;
            }
            await Current.Dispatcher.InvokeAsync(action);
        }

        private static Task ShowMainWindowAsync() => OnUiThreadAsync(() =>
        {
            var mainWindow = Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                if (!mainWindow.IsVisible)
                    mainWindow.Show();
                else if (mainWindow.WindowState == WindowState.Minimized)
                    mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            }
        });

        private static Task FinishCodeExchange(string[] args) => OnUiThreadAsync(() =>
        {
            var manager = provider.GetService(typeof(OAuthManager)) as OAuthManager;
            manager.ProcessUriAndExchangeCode(args);
        });

        public App() : base()
        {
            singleInstance = new SingleInstance(APP_NAME, SecondaryInstanceMessageReceivedHandler);
        }

        readonly SingleInstance singleInstance;

        async void Initialize(object startupObject)
        {
            var args = (StartupEventArgs)startupObject;

            if (!singleInstance.IsFirstInstance)
            {
                await singleInstance.SendMessageAsync(args.Args);
                Environment.Exit(0);
            }

            // Finish starting your app (you'll need to use Current.Dispatcher to get back on the UI thread)
            var services = new ServiceCollection();

            var config = new Destiny2.Destiny2Config(APP_NAME, "1.0", "30077", "", "");
            config.ApiKey = "c50bb382b1b84e1ba9640125c8f8f299";

            services.AddDestiny2(config);
            services.AddSingleton(new OAuthManager("30077", "55593"));
            var accountManager = new AccountManager();
            services.AddSingleton(accountManager);
            var manifestManager = new ManifestManager();
            services.AddSingleton(manifestManager);

            provider = services.BuildServiceProvider();

            accountManager.SetupServices();
            manifestManager.SetupServices();

            _ = manifestManager.DownloadManifest();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(Initialize, e);
            base.OnStartup(e);
        }

        async Task SecondaryInstanceMessageReceivedHandler(object message)
        {
            var messageObject = (JToken)message;
            var args = messageObject.Values<string>();
            System.Diagnostics.Debug.WriteLine(args);

            await FinishCodeExchange(args.ToArray());
            await ShowMainWindowAsync();

            // Don't forget that you will be on a worker pool thread in here (as opposed to the UI thread)
            switch (message)
            {
                case "showmainwindow":
                    break;
            }
        }
    }
}
