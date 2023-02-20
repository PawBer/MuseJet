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
        private Guid _id;
        private string _name;
        private string _url;
        private string? _imageUrl;

        public EditStationViewModel(StationService service, Station station)
        {
            _service = service;

            _id = station.Id;
            _name = station.Name;
            _url = station.Url;
            _imageUrl = station.ImageUrl;

            SubmitCommand = new RelayCommand((obj) =>
            {
                _service.Edit(new Station() { Id = _id, Name = _name, Url = _url, ImageUrl = _imageUrl});
                OnRequestClose();
            }, null);
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
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

        public string? ImageUrl
        {
            get => _imageUrl;
            set
            {
                _imageUrl = value;
                OnPropertyChanged();
            }
        }

        public ICommand SubmitCommand { get; set; }

        public event EventHandler? RequestClose;

        public void OnRequestClose()
        {
            RequestClose?.Invoke(this, new EventArgs());
        }
    }
}
