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

namespace MuseJet.GUI.ViewModels
{
    public class MainWindowViewModel : IDisposable, INotifyPropertyChanged
    {
        public static Station EmptyStation = new() { Name = "Select a Station", Url = "" };
        public ConfigService Config { get; }
        public StationPlayer? StationPlayer { get; set; } = null;
        public float Volume
        {
            get
            {
                return Config.Volume;
            }
            set
            {
                if (StationPlayer == null)
                {
                    Config.Volume = value;
                    return;
                }
                Config.Volume = value;
                StationPlayer.Volume = value;
                OnPropertyChanged();
            }
        }
        private Station _currentStation;
        public Station CurrentStation
        {
            get => _currentStation;
            set
            {
                _currentStation = value;
                OnPropertyChanged();
            }
        }

        private StationService _stationService;
        private ObservableCollection<Station> _stationList;
        public ObservableCollection<Station> StationList
        {
            get => _stationList;
            set
            {
                _stationList = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            Config = new();
            _stationService = new();
            StationList = new(_stationService.GetAll().OrderBy(s => s.Name));
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
                AddStationView StationAddWindow = new(this);
                StationAddWindow.ShowDialog();
            }, null);

            DeleteStationCommand = new RelayCommand((obj) =>
            {
                if (StationPlayer != null) Stop();
                RemoveStation(CurrentStation);
                CurrentStation = EmptyStation;
            },
            (obj) => CurrentStation.Name != EmptyStation.Name);

            EditStationCommand = new EditStationCommand(this);
        }

        public void ChangeStation(Station station)
        {
            CurrentStation = station;
        }

        public void Play(object? sender = null)
        {
            if (StationPlayer != null && StationPlayer.GetState() == PlaybackState.Paused) StationPlayer.Play();
            if (StationPlayer != null) StationPlayer.Dispose();

            try
            {
                StationPlayer = new(CurrentStation);
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

        public void AddStation(Station station)
        {
            _stationService.Add(station);
            StationList = new(_stationService.GetAll().OrderBy(s => s.Name));
        }

        public void RemoveStation(Station station)
        {
            _stationService.Remove(station);
            StationList.Remove(station);
        }

        public void EditStation(Station station)
        {
            _stationService.Edit(station);
            StationList = new(_stationService.GetAll().OrderBy(s => s.Name));
            CurrentStation = station;
            Stop();
        }

        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand AddStationCommand { get; set; }
        public ICommand DeleteStationCommand { get; set; }
        public ICommand EditStationCommand { get; set; }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

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
