using MuseJet.Common.Services;
using MuseJet.GUI.Events;
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
        private ObservableCollection<SearchItemViewModel> _searchResults;
        private bool _isLoading;
        private uint _count = 100;

        public ObservableCollection<SearchItemViewModel> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public SearchViewModel(RadioBrowserClient browser, StationService service)
        {
            _browser = browser;
            _service = service;
            _searchResults = new();
            OnGetToEnd();

            _service.StationStateChanged += OnStationStateChanged;
        }

        public void OnStationStateChanged(object? sender, EventArgs args)
        {
            StationStateChangeEventArgs parameter = (StationStateChangeEventArgs)args;
            if (parameter.Type == ChangeType.Add)
            {
                SearchResults.Remove(SearchResults.First(s => s.Id == parameter.ChangedStation.Id));
            }
        }

        public void OnGetToEnd()
        {
            IsLoading = true;
            Task.Run(async () =>
            {
                var results = await _browser.Stations.GetByVotesAsync(_count);
                _count += 100;
                SearchResults = new(results.Select(s => new SearchItemViewModel(_service, s)).Where(s => s.Visible));
                IsLoading = false;
            });
        }
    }
}
