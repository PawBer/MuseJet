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

namespace MuseJet.GUI.ViewModels
{
    public class MainWindowViewModel : IDisposable, INotifyPropertyChanged
    {
        public StationPlayer? StationPlayer { get; set; } = null;
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
        public ObservableCollection<Station> StationList { get; set; }

        public MainWindowViewModel()
        {
            _stationService = new();
            StationList = new(_stationService.GetAll());
            CurrentStation = new() { Name = "Select a Station", Url = "" };

            PlayCommand = new RelayCommand((obj) => { StationPlayer.Play(); },
                (obj) => StationPlayer != null && (StationPlayer.GetState() != PlaybackState.Playing));

            PauseCommand = new RelayCommand((obj) => { StationPlayer.Pause(); },
                (obj) => StationPlayer != null && (StationPlayer.GetState() == PlaybackState.Playing));

            StopCommand = new RelayCommand((obj) => { StationPlayer.Stop(); },
                (obj) => StationPlayer != null && (StationPlayer.GetState() != PlaybackState.Stopped));

            ChangeStationCommand = new ChangeStationCommand(this);

            AddStationCommand = new RelayCommand((obj) =>
            {
                AddStationView StationAddWindow = new(this);
                StationAddWindow.ShowDialog();
            }, null);

            DeleteStationCommand = new RelayCommand((obj) =>
            {
                RemoveStation(CurrentStation);
                StationPlayer.Dispose();
                StationPlayer = null;
                CurrentStation = new() { Name = "Select a Station", Url = "" };
            },
            (obj) => !CurrentStation.Name.Equals("Select a Station"));

            EditStationCommand = new EditStationCommand(this);
        }

        public void ChangeStation(Station station)
        {
            if (StationPlayer != null) StationPlayer.Dispose();
            StationPlayer = new(station);
            CurrentStation = station;
        }

        public void AddStation(Station station)
        {
            _stationService.Add(station);
            StationList.Add(station);
        }

        public void RemoveStation(Station station)
        {
            _stationService.Remove(station);
            StationList.Remove(station);
        }

        public void EditStation(Station station)
        {
            _stationService.Edit(station);
            StationList = new ObservableCollection<Station>(_stationService.GetAll());
            CurrentStation = station;
        }

        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand ChangeStationCommand { get; set; }
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
