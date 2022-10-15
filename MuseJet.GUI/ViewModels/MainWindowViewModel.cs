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
using NAudio.Wave;
using MuseJet.GUI.Views;

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

        public MainWindowViewModel()
        {
            CurrentStation = new() { Name = "Select a Station", Url = "" };
            PlayCommand = new RelayCommand((obj) => { StationPlayer.Play(); }, (obj) => StationPlayer == null ? false : (StationPlayer.GetState() != PlaybackState.Playing));
            PauseCommand = new RelayCommand((obj) => { StationPlayer.Pause(); }, (obj) => StationPlayer == null ? false : (StationPlayer.GetState() == PlaybackState.Playing));
            StopCommand = new RelayCommand((obj) => { StationPlayer.Stop(); }, (obj) => StationPlayer == null ? false : (StationPlayer.GetState() != PlaybackState.Stopped));
            ChangeStationCommand = new RelayCommand((obj) =>
            {
                StationChangeView StationChangeWindow = new(this);
                StationChangeWindow.ShowDialog();
            }, null);
        }

        public void ChangeStation(Station station)
        {
            if (StationPlayer != null) StationPlayer.Dispose(); 

            StationPlayer = new(station);
        }

        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand ChangeStationCommand { get; set; }

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
