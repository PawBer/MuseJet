using MuseJet.Common.Services;
using RadioBrowser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MuseJet.GUI.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly RadioBrowserClient _browser;
        private readonly StationService _service;

        public ObservableCollection<SearchItemViewModel> SearchResults { get; set; }

        public SearchViewModel(RadioBrowserClient browser, StationService service)
        {
            _browser = browser;
            _service = service;

            Task.Run(async () =>
            {
                var results = await _browser.Search.ByNameAsync("Epic Rock Radio");
                SearchResults = new(results.Select(s => new SearchItemViewModel(_service, s)));
            });

            Thread.Sleep(5000);
            Debug.Print("");
        }
    }
}
