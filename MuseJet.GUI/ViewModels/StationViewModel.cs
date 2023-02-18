using MuseJet.Common.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MuseJet.GUI.ViewModels
{
    public class StationViewModel : ViewModelBase
    {
        public readonly Station Station;

        public string Name => Station.Name;
        public string Url => Station.Url;
        public BitmapImage Icon
        {
            get
            {
                if (Station.ImageUrl is null)
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/note-icon.jpg"));
                else
                    return new BitmapImage(new Uri(Station.ImageUrl));
            }
        }

        public StationViewModel(Station station)
        {
            Station = station;
        }
    }
}
