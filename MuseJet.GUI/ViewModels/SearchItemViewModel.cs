using MuseJet.Common.Models;
using MuseJet.Common.Services;
using MuseJet.GUI.Commands;
using RadioBrowser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MuseJet.GUI.ViewModels
{
    public class SearchItemViewModel : ViewModelBase
    {
        private readonly StationService _service;

        private readonly string _name;
        private readonly string _url;
        private readonly string _imageUrl;
        private readonly string _language;
        private readonly string _genre;
        private bool _visible;

        public string Name => _name;
        public string Url => _url;
        public string ImageUrl => _imageUrl;
        public string Language => _language;
        public string Genre => _genre;
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                OnPropertyChanged();
            }
        }

        public SearchItemViewModel(StationService service, StationInfo station)
        {
            _service = service;

            _name = station.Name;
            _url = station.UrlResolved.ToString();
            _imageUrl = station.Favicon?.ToString();
            _language = station.Language?.First();
            _genre = station.Tags.First();
            Visible = !_service.Exists(station.StationUuid);

            AddStationCommand = new RelayCommand((obj) =>
            {
                Station newStation = new() {Id = station.StationUuid,  Name = _name, Url = _url, ImageUrl = _imageUrl };
                _service.Add(newStation);
            }, null);
        }

        public ICommand AddStationCommand { get; set; }
    }
}
