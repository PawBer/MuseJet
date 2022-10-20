using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuseJet.Common.Models;
using Microsoft.Data.Sqlite;

namespace MuseJet.Common.Services
{
    public class StationService : IDisposable
    {
        private string _userFiles = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MuseJet");
        private SqliteConnection _dbConnection;

        public StationService()
        {
            if (!Directory.Exists(_userFiles)) Directory.CreateDirectory(_userFiles);

            string dbPath = Path.Combine(_userFiles, "station.db");
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
        }

        public void Remove(Station station)
        {
            string statement = "DELETE FROM stations WHERE name=@name;";
            SqliteCommand command = new(statement, _dbConnection);

            command.Parameters.AddWithValue("@name", station.Name);

            command.Prepare();
            command.ExecuteNonQuery();
        }

        public void Edit(Station station)
        {
            string statement = "UPDATE stations SET url=@url WHERE name=@name;";
            SqliteCommand command = new(statement, _dbConnection);

            command.Parameters.AddWithValue("@name", station.Name);
            command.Parameters.AddWithValue("@url", station.Url);

            command.Prepare();
            command.ExecuteNonQuery();
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
