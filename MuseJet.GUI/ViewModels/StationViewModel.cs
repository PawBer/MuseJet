using MuseJet.Common.Models;
using MuseJet.Common.Services;
using MuseJet.GUI.Commands;
using MuseJet.GUI.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MuseJet.GUI.ViewModels
{
    public class StationViewModel : ViewModelBase
    {
        public readonly Station Station;
        private readonly StationService _service;

        public string Name => Station.Name;
        public string Url => Station.Url;
        public BitmapImage Icon
        {
            get
            {
                if (Station.ImageUrl is null)
                {
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/note-icon.jpg"));
                }
                else
                {
                    //TODO Check if response leads to image
                    return new BitmapImage(new Uri(Station.ImageUrl));
                }
            }
        }

        public StationViewModel(StationService service, Station station)
        {
            _service = service;
            Station = station;

            EditCommand = new RelayCommand((obj) =>
            {
                EditStationViewModel vm = new(_service, Station);
                EditStationView view = new()
                {
                    DataContext = vm
                };
                vm.RequestClose += (o, a) => view.Close();
                view.ShowDialog();
            }, null);

            DeleteCommand = new RelayCommand((obj) =>
            {
                _service.Remove(station);
            }, null);
        }


        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
    }
}
