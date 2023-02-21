using MuseJet.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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

        public BitmapImage Icon
        {
            get
            {              
                if (_currentStation?.ImageUrl is null)
                {
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/note-icon.jpg"));
                }
                else
                {
                    //TODO Check if response leads to image
                    return new BitmapImage(new Uri(_currentStation?.ImageUrl));
                }
            }
        }

        public event Action? CurrentStationChanged;

        public void OnCurrentStationChanged()
        {
            CurrentStationChanged?.Invoke();
        }
    }
}
