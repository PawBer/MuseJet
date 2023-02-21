using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MuseJet.Common.Services;
using MuseJet.GUI.HostBuilders;
using MuseJet.GUI.Services;
using MuseJet.GUI.State;
using MuseJet.GUI.Stores;
using MuseJet.GUI.ViewModels;
using RadioBrowser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace MuseJet.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .AddViewModels()
                .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<ConfigService>();
                services.AddSingleton<StationService>();
                services.AddSingleton<RadioBrowserClient>();

                services.AddSingleton<NavigationState>();
                services.AddSingleton<StationState>();

                services.AddSingleton(s => new MainWindow()
                {
                    DataContext = s.GetRequiredService<MainViewModel>()
                });
            }).Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            NavigationService<PlayerViewModel> navigationService = _host.Services.GetRequiredService<NavigationService<PlayerViewModel>>();
            navigationService.Navigate();

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();

            MainWindow.Closing += (obj, e) => {
                _host.Services.GetRequiredService<ConfigService>().Save();
            };

            base.OnStartup(e);
        }
    }

}
