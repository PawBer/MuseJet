using MuseJet.Common.Models;
using MuseJet.GUI.Commands;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MuseJet.GUI.Views;
using MuseJet.Common.Services;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using MuseJet.GUI.Events;

namespace MuseJet.GUI.ViewModels
{
    public class PlayerViewModel : ViewModelBase, IDisposable
    {
        public static StationViewModel EmptyStation = new(null, new Station() {Id = new(), Name = "Select a Station", Url = "", ImageUrl = null });
        private ConfigService _config { get; }
        private StationService _stationService;
        public StationPlayer? StationPlayer { get; set; } = null;
        public float Volume
        {
            get => _config.Volume;
            set
            {
                if (StationPlayer == null)
                {
                    _config.Volume = value;
                    return;
                }
                _config.Volume = value;
                StationPlayer.Volume = value;
                OnPropertyChanged();
            }
        }
        private StationViewModel _currentStation;
        public StationViewModel CurrentStation
        {
            get => _currentStation;
            set
            {
                _currentStation = value;
                OnPropertyChanged();
                OnCurrentStationChanged();
            }
        }

        private ObservableCollection<StationViewModel> _stationList;
        public ObservableCollection<StationViewModel> StationList
        {
            get => _stationList;
            set
            {
                _stationList = value;
                OnPropertyChanged();
            }
        }

        public PlayerViewModel(StationService stationService, ConfigService configService)
        {
            _stationService = stationService;
            _config = configService;

            StationList = new(_stationService.GetAll()
                .OrderBy(s => s.Name)
                .Select(s => new StationViewModel(_stationService, s)));

            CurrentStation = EmptyStation;

            PlayCommand = new RelayCommand(Play,
                (obj) =>
                {
                    if (CurrentStation.Name == EmptyStation.Name)
                        return false;
                    else if (StationPlayer == null && CurrentStation.Name != EmptyStation.Name)
                        return true;
                    else if (StationPlayer.GetState() != PlaybackState.Playing)
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

            _stationService.StationStateChanged += ((obj, args) =>
            {
                StationStateChangeEventArgs changeArgs = (StationStateChangeEventArgs)args;
                switch (changeArgs.Type)
                {
                    case ChangeType.Add:
                        StationList.Add(new StationViewModel(_stationService, changeArgs.ChangedStation));
                        break;
                    case ChangeType.Edit:
                        StationList.Remove(StationList.First(x => x.Id == changeArgs.ChangedStation.Id));
                        StationList.Add(new StationViewModel(_stationService, changeArgs.ChangedStation));
                        StationList.OrderBy(s => s.Name);
                        CurrentStation = new StationViewModel(_stationService, changeArgs.ChangedStation);
                        break;
                    case ChangeType.Delete:
                        StationList.Remove(StationList.First(x => x.Id == changeArgs.ChangedStation.Id));
                        CurrentStation = EmptyStation;
                        break;
                }
            });
        }

        public void OnCurrentStationChanged()
        {
            if (CurrentStation == EmptyStation)
                return;
            else
                Play();
        }

        public void Play(object? sender = null)
        {
            if (StationPlayer != null && StationPlayer.GetState() == PlaybackState.Paused) StationPlayer.Play();
            if (StationPlayer != null) StationPlayer.Dispose();

            try
            {
                StationPlayer = new(CurrentStation.Station);
                StationPlayer.Play();
                StationPlayer.Volume = Volume;
            }
            catch
            {
                MessageBox.Show("The URL doesn't lead to an audio file.\nYou should delete or edit it.",
                    "Bad URL",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                StationPlayer = null;
            }
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


        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand AddStationCommand { get; set; }

        #region Disposal

        public bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (StationPlayer == null) return;
                StationPlayer.Dispose();
                _stationService.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
