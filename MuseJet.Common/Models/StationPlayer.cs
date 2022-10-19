using NAudio.CoreAudioApi;
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
        private DirectSoundOut _directOut;
        private WaveChannel32 _waveChannel;
        private float _volume = 0.3F;

        public float Volume
        {
            get => _volume;
            set {
                if (value < 0.0F && value > 1.0F)
                {
                    _waveChannel.Volume = 0.0F;
                    _volume = 0.0F;
                } else
                {
                    _waveChannel.Volume = value;
                    _volume = value;
                }
            }
        }

        public StationPlayer(Station station)
        {
            try
            {
                _mediaFoundationReader = new(station.Url);
                _directOut = new();
                _waveChannel = new(_mediaFoundationReader);
                _directOut.Init(_waveChannel);
                _waveChannel.Volume = _volume;
            } catch
            {
                throw new Exception();
            }
            
        }

        public PlaybackState GetState() => _directOut.PlaybackState;

        public void Play() => _directOut.Play();

        public void Pause() => _directOut.Pause();

        public void Stop() => _directOut.Stop();

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
                _directOut.Dispose();
                _waveChannel.Dispose();
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
