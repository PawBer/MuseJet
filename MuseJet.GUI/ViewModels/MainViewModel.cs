using MuseJet.Common.Models;
using MuseJet.Common.Services;
using MuseJet.GUI.Commands;
using MuseJet.GUI.Services;
using MuseJet.GUI.State;
using MuseJet.GUI.Stores;
using MuseJet.GUI.Views;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MuseJet.GUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationService<PlayerViewModel> _goToPlayer;
        private readonly NavigationService<SearchViewModel> _goToSearch;

        private readonly ConfigService _configService;
        private readonly StationService _stationService;
        private readonly NavigationState _navigationState;
        private readonly StationState _stationState;

        public ViewModelBase CurrentViewModel => _navigationState.CurrentViewModel;
        public StationPlayer? StationPlayer { get; set; } = null;
        public float Volume
        {
            get => _configService.Volume;
            set
            {
                if (StationPlayer is null)
                {
                    _configService.Volume = value;
                    return;
                }
                _configService.Volume = value;
                StationPlayer.Volume = value;
                OnPropertyChanged();
            }
        }
        public Station? CurrentStation
        {
            get => _stationState.CurrentStation;
        }

        public string CurrentStationName
        {
            get => _stationState.CurrentStation?.Name ?? "Select a station";
        }

        public BitmapImage Icon
        {
            get => _stationState.Icon;
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnLoadingChanged();
            }
        }

        public MainViewModel(NavigationState navigationState,
                             StationState stationState,
                             StationService stationSerivce,
                             ConfigService configService,
                             NavigationService<PlayerViewModel> goToPlayer,
                             NavigationService<SearchViewModel> goToSearch)
        {
            _navigationState = navigationState;
            _goToPlayer = goToPlayer;
            _goToSearch = goToSearch;

            _configService = configService;
            _stationService = stationSerivce;
            _stationState = stationState;

            ChangeViewModelCommand = new RelayCommand((obj) =>
            {
                NavDest parameter = (NavDest)obj;
                switch (parameter)
                {
                    case NavDest.Player:
                        goToPlayer.Navigate();
                        break;
                    case NavDest.Search:
                        goToSearch.Navigate();
                        break;
                }
            }, null);

            PlayCommand = new RelayCommand(Play,
                (obj) =>
                {
                    if (IsLoading)
                        return false;
                    else if (CurrentStation is null)
                        return false;
                    else if (StationPlayer == null && CurrentStation is not null)
                        return true;
                    else if (StationPlayer!.GetState() != PlaybackState.Playing)
                        return true;
                    else
                        return false;
                });

            PauseCommand = new RelayCommand(Pause,
                (obj) => StationPlayer != null && (StationPlayer.GetState() == PlaybackState.Playing));

            StopCommand = new RelayCommand(Stop,
                (obj) => StationPlayer != null && (StationPlayer.GetState() != PlaybackState.Stopped));

            AddStationCommand = new RelayCommand((obj) =>
            {
                AddStationViewModel vm = new(_stationService);
                AddStationView view = new()
                {
                    DataContext = vm
                };
                vm.RequestClose += (o, a) => view.Close();
                view.ShowDialog();
            }, null);


            _stationState.CurrentStationChanged += OnCurrentStationChanged;
            _navigationState.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        public void OnCurrentStationChanged()
        {
            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(CurrentStationName));
            if (CurrentStation is null)
                Stop();
            else
                Play();
        }

        public void OnLoadingChanged()
        {
            OnPropertyChanged(nameof(IsLoading));
            if (IsLoading == false)
            {
                Application.Current.Dispatcher.Invoke(() => { CommandManager.InvalidateRequerySuggested(); });
            }
        }

        public void Play(object? sender = null)
        {
            if (CurrentStation is null) return;
            if (StationPlayer is not null && StationPlayer.GetState() == PlaybackState.Paused)
            {
                StationPlayer.Play();
                return;
            }
            StationPlayer?.Dispose();

            IsLoading = true;
            Task.Run(() =>
            {
                try
                {
                    StationPlayer = new((Station)CurrentStation);
                    StationPlayer.Play();
                    StationPlayer.Volume = Volume;
                    IsLoading = false;
                }
                catch
                {
                    MessageBox.Show("The URL doesn't lead to an audio file.\nYou should delete or edit it.",
                        "Bad URL",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    StationPlayer = null;
                }
            });
        }

        public void Pause(object? sender = null)
        {
            if (StationPlayer == null) return;
            StationPlayer.Pause();
        }

        public void Stop(object? sender = null)
        {
            if (StationPlayer == null) return;
            StationPlayer.Dispose();
            StationPlayer = null;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public ICommand ChangeViewModelCommand { get; set; }
        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand AddStationCommand { get; set; }
    }
}
