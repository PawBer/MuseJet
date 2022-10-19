using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuseJet.Common.Models;

namespace MuseJet.Common.Services
{
    public class StationService
    {
        private List<Station> _stations;

        public StationService()
        {
            _stations = new();
            _stations.Add(new Station() {Name = "Radio Plus Radom", Url = "http://pl01.cdn.eurozet.pl/plu-rdo.mp3" });
            _stations.Add(new Station() {Name = "RMF FM", Url = "http://195.150.20.244/rmf_fm" });
        }

        public IEnumerable<Station> GetAll() => _stations;

        public void Add(Station station)
        {
            _stations.Add(station);
        }

        public void Remove(Station station)
        {
            var stationToRemove = _stations.First(s => s.Name == station.Name);
            _stations.Remove(stationToRemove);
        }

        public void Edit(Station station)
        {
            var oldStation = _stations.First(s => s.Name == station.Name);
            _stations.Remove(oldStation);
            _stations.Add(station);
        }
    }
}
