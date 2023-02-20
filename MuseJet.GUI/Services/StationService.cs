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

            string createTableQueryText = "CREATE TABLE IF NOT EXISTS stations(id TEXT NOT NULL PRIMARY KEY, name TEXT NOT NULL, url TEXT NOT NULL, image_url TEXT);";

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
                Guid guid = new(reader.GetString(0));
                string name = reader.GetString(1);
                string url = reader.GetString(2);
                string? imageUrl = null;
                if (!reader.IsDBNull(3))
                    imageUrl = reader.GetString(3);
                Station station = new() {Id = guid, Name = name, Url = url, ImageUrl = imageUrl };
                stations.Add(station);
            }

            return stations;
        }

        public bool Exists(Guid id)
        {
            string statement = "SELECT 1 FROM stations WHERE id=@id";

            SqliteCommand command = new(statement, _dbConnection);
            command.Parameters.AddWithValue("@id", id.ToString());

            command.Prepare();
            SqliteDataReader reader = command.ExecuteReader();

            return reader.Read();
        }

        public void Add(Station station)
        {
            string statement;

            if (station.ImageUrl is not null)
                statement = "INSERT INTO stations(name, url, image_url) VALUES(@name, @url, @image_url);";
            else
                statement = "INSERT INTO stations(name, url) VALUES(@name, @url);";

            SqliteCommand command = new(statement, _dbConnection);

            command.Parameters.AddWithValue("@name", station.Name);
            command.Parameters.AddWithValue("@url", station.Url);

            if (station.ImageUrl is not null)
                command.Parameters.AddWithValue("@image_url", station.ImageUrl);

            command.Prepare();
            command.ExecuteNonQuery();

            OnStationStateChanged(ChangeType.Add, station);
        }

        public void Remove(Station station)
        {
            string statement = "DELETE FROM stations WHERE id=@id;";
            SqliteCommand command = new(statement, _dbConnection);

            command.Parameters.AddWithValue("@name", station.Id.ToString());

            command.Prepare();
            command.ExecuteNonQuery();

            OnStationStateChanged(ChangeType.Delete, station);
        }

        public void Edit(Station station)
        {
            string statement;

            if (station.ImageUrl is not null)
                statement = "UPDATE stations SET name=@name, url=@url, image_url=@image_url WHERE id=@id;";
            else
                statement = "UPDATE stations SET name=@name, url=@url WHERE id=@id;";

            SqliteCommand command = new(statement, _dbConnection);

            command.Parameters.AddWithValue("@id", station.Id.ToString());
            command.Parameters.AddWithValue("@name", station.Name);
            command.Parameters.AddWithValue("@url", station.Url);

            if (station.ImageUrl is not null)
                command.Parameters.AddWithValue("@image_url", station.ImageUrl);

            command.Prepare();
            command.ExecuteNonQuery();

            OnStationStateChanged(ChangeType.Edit, station);

        }

        public event EventHandler StationStateChanged;

        protected virtual void OnStationStateChanged(ChangeType type, Station station)
        {
            StationStateChanged?.Invoke(this, new StationStateChangeEventArgs() { Type = type, ChangedStation = station });
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
