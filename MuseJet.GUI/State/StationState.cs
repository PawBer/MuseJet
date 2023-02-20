using MuseJet.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseJet.GUI.State
{
    public class StationState
    {
        private Station? _currentStation;
        public Station? CurrentStation
        {
            get => _currentStation;
            set
            {
                _currentStation = value;
                OnCurrentStationChanged();
            }
        }

        public event Action? CurrentStationChanged;

        public void OnCurrentStationChanged()
        {
            CurrentStationChanged?.Invoke();
        }
    }
}
