using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseJet.Common.Models
{
    public class StationPlayer : IDisposable
    {
        private MediaFoundationReader _mediaFoundationReader;
        private WasapiOut _wasapiOut;
        private float _volume = 0.01F;

        public float Volume
        {
            get => _volume;
            set {
                if (value < 0.0F && value > 1.0F)
                {
                    _wasapiOut.Volume = 0.0F;
                    _volume = 0.0F;
                } else
                {
                    _wasapiOut.Volume = value;
                    _volume = value;
                }
            }
        }

        public StationPlayer(Station station)
        {
            _mediaFoundationReader = new(station.Url);
            _wasapiOut = new();
            _wasapiOut.Init(_mediaFoundationReader);
            _wasapiOut.Volume = _volume;
        }

        public PlaybackState GetState() => _wasapiOut.PlaybackState;

        public void Play() => _wasapiOut.Play();

        public void Pause() => _wasapiOut.Pause();

        public void Stop() => _wasapiOut.Stop();

        #region Disposal
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _mediaFoundationReader.Dispose();
                _wasapiOut.Dispose();
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
