using MuseJet.Common.Models;
using MuseJet.Common.Services;
using MuseJet.GUI.Commands;
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
using System.Windows.Input;

namespace MuseJet.GUI.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly RadioBrowserClient _browser;
        private readonly StationService _service;
        private ObservableCollection<SearchItemViewModel> _searchResults;
        private bool _isLoading;
        private bool _isSearching;
        private string _searchTerm;
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

        public bool IsSearching
        {
            get => _isSearching;
            set
            {
                _isSearching = value;
                OnPropertyChanged();
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
            }
        }

        public SearchViewModel(RadioBrowserClient browser, StationService service)
        {
            _browser = browser;
            _service = service;
            _isSearching = false;
            _searchResults = new();
            OnGetToEnd();

            SubmitSearchCommand = new RelayCommand(async (obj) =>
            {
                IsSearching = true;
                IsLoading = true;
                var results = await _browser.Search.ByNameAsync(SearchTerm);
                SearchResults = new(results.Select(s => new SearchItemViewModel(_service, s)).Where(s => s.Visible));
                IsLoading = false;
            }, null);

            BackCommand = new RelayCommand((obj) =>
            {
                IsLoading = true;
                SearchResults = new();
                SearchTerm = string.Empty;
                IsSearching = false;
                _count = 100;
                OnGetToEnd();
            }, null);

            _service.StationStateChanged += OnStationStateChanged;
        }

        public void OnStationStateChanged(object? sender, EventArgs args)
        {
            StationStateChangeEventArgs parameter = (StationStateChangeEventArgs)args;
            if (parameter.Type == ChangeType.Add)
            {
                SearchItemViewModel? station = SearchResults.FirstOrDefault(s => s.Id == parameter.ChangedStation.Id);
                if (station is null)
                    return;
                else
                    SearchResults.Remove(station);
            }
        }

        public void OnGetToEnd()
        {
            if (IsSearching)
                return;
            IsLoading = true;
            Task.Run(async () =>
            {
                var results = await _browser.Stations.GetByVotesAsync(_count);
                _count += 100;
                SearchResults = new(results.Select(s => new SearchItemViewModel(_service, s)).Where(s => s.Visible));
                IsLoading = false;
            });
        }

        public ICommand SubmitSearchCommand { get; set; }
        public ICommand BackCommand { get; set; }
    }
}
