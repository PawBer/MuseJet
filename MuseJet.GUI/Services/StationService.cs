using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuseJet.Common.Models;
using Microsoft.Data.Sqlite;
using MuseJet.GUI.Events;
using System.IO;

namespace MuseJet.Common.Services
{
    public class StationService : IDisposable
    {
        private SqliteConnection _dbConnection;

        public StationService()
        {
            string dbPath = Path.Combine(ConfigService.UserFilesDir, "station.db");
            string connectionString = $"Data Source={dbPath}";

            _dbConnection = new(connectionString);
            _dbConnection.Open();

            string createTableQueryText = "CREATE TABLE IF NOT EXISTS stations(name TEXT PRIMARY KEY, url TEXT NOT NULL);";

            SqliteCommand command = new(createTableQueryText, _dbConnection);
            command.ExecuteNonQuery();
        }

        public IEnumerable<Station> GetAll()
        {
            List<Station> stations = new();
            string statement = "SELECT * FROM stations;";

            SqliteCommand command = new(statement, _dbConnection);
            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Station station = new() { Name = reader.GetString(0), Url = reader.GetString(1) };
                stations.Add(station);
            }

            return stations;
        }

        public void Add(Station station)
        {
            string statement = "INSERT INTO stations(name, url) VALUES(@name, @url);";
            SqliteCommand command = new(statement, _dbConnection);

            command.Parameters.AddWithValue("@name", station.Name);
            command.Parameters.AddWithValue("@url", station.Url);

            command.Prepare();
            command.ExecuteNonQuery();

            OnStationStateChanged(ChangeType.Add, station);
        }

        public void Remove(Station station)
        {
            string statement = "DELETE FROM stations WHERE name=@name;";
            SqliteCommand command = new(statement, _dbConnection);

            command.Parameters.AddWithValue("@name", station.Name);

            command.Prepare();
            command.ExecuteNonQuery();

            OnStationStateChanged(ChangeType.Delete, station);
        }

        public void Edit(Station station)
        {
            string statement = "UPDATE stations SET url=@url WHERE name=@name;";
            SqliteCommand command = new(statement, _dbConnection);

            command.Parameters.AddWithValue("@name", station.Name);
            command.Parameters.AddWithValue("@url", station.Url);

            command.Prepare();
            command.ExecuteNonQuery();

            OnStationStateChanged(ChangeType.Edit, station);
        }

        public event EventHandler StationStateChanged;

        protected virtual void OnStationStateChanged(ChangeType type, Station station)
        {
            StationStateChanged?.Invoke(this, new StationStateChangeEventArgs() { Type = type, ChangedStation = station});
        }

        #region Disposal

        public bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_dbConnection == null) return;
                _dbConnection.Close();
                _dbConnection.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
