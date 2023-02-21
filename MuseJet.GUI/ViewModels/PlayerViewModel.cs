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
using MuseJet.GUI.State;

namespace MuseJet.GUI.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly StationService _stationService;
        private readonly StationState _stationState;

        private StationViewModel? _currentStation;
        public StationViewModel? CurrentStation
        {
            get => _currentStation;
            set
            {
                _currentStation = value;
                _stationState.CurrentStation = value?.Station;
                OnPropertyChanged();
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

        public PlayerViewModel(StationService stationService, StationState stationState)
        {
            _stationService = stationService;
            _stationState = stationState;

            _stationList = new(_stationService.GetAll()
                .OrderBy(s => s.Name)
                .Select(s => new StationViewModel(_stationService, s)));

            _stationService.StationStateChanged += ((obj, args) =>
            {
                StationStateChangeEventArgs changeArgs = (StationStateChangeEventArgs)args;
                StationViewModel newModel = new StationViewModel(_stationService, changeArgs.ChangedStation);
                switch (changeArgs.Type)
                {
                    case ChangeType.Add:
                        StationList.Add(newModel);
                        StationList = new(StationList.OrderBy(s => s.Name));
                        break;
                    case ChangeType.Edit:
                        StationList.Remove(StationList.First(x => x.Id == changeArgs.ChangedStation.Id));
                        StationList.Add(newModel);
                        StationList = new(StationList.OrderBy(s => s.Name));
                        CurrentStation = null;
                        break;
                    case ChangeType.Delete:
                        StationList.Remove(StationList.First(x => x.Id == changeArgs.ChangedStation.Id));
                        if (changeArgs.ChangedStation.Id == CurrentStation?.Id)
                            CurrentStation = null;
                        break;
                }
            });
        }
    }
}
