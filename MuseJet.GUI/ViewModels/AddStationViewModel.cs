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
    public class AddStationViewModel : ViewModelBase
    {
        private StationService _service;

        public AddStationViewModel(StationService service)
        {
            _service = service;
            SubmitCommand = new RelayCommand((obj) =>
            {
                if (_imageUrl == String.Empty)              
                    _service.Add(new Station() { Id = Guid.NewGuid(), Name = Name, Url = Url, ImageUrl = null });               
                else              
                    _service.Add(new Station() { Id = Guid.NewGuid(), Name = Name, Url = Url, ImageUrl = _imageUrl });
                OnRequestClose();
            }, null);
        }

        private string _name = String.Empty;
        private string _url = String.Empty;
        private string _imageUrl = String.Empty;

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
