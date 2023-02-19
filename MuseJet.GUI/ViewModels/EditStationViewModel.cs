using MuseJet.Common.Models;
using MuseJet.Common.Services;
using MuseJet.GUI.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MuseJet.GUI.ViewModels
{
    public class EditStationViewModel : ViewModelBase
    {
        private StationService _service;
        private string _name;
        private string _url;
        private string _imageUrl;

        public EditStationViewModel(StationService service, Station station)
        {
            _service = service;

            _name = station.Name;
            _url = station.Url;
            _imageUrl = station.ImageUrl;

            SubmitCommand = new RelayCommand((obj) =>
            {
                _service.Edit(station);
                OnRequestClose();
            }, null);
        }

        public string Name
        {
            get => _name;
        }

        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                _imageUrl = value;
                OnPropertyChanged();
            }
        }

        public ICommand SubmitCommand { get; set; }

        public event EventHandler RequestClose;

        public void OnRequestClose()
        {
            RequestClose?.Invoke(this, new EventArgs());
        }
    }
}
