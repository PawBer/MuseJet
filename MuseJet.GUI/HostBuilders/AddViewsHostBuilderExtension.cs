using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MuseJet.GUI.Services;
using MuseJet.GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace MuseJet.GUI.HostBuilders
{
    public static class AddViewsHostBuilderExtension
    {
        public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                services.AddTransient<PlayerViewModel>();
                services.AddSingleton<Func<PlayerViewModel>>((s) => () => s.GetRequiredService<PlayerViewModel>());
                services.AddSingleton<NavigationService<PlayerViewModel>>();

                services.AddSingleton<MainViewModel>();
            });

            return hostBuilder;
        }
 
    }
}
