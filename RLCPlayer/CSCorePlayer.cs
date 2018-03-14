using System;
using System.ComponentModel;
using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;

namespace RLCPlayer
{
    public class CSMusicPlayer: Component
    {
        private ISoundOut _soundOut;
        private IWaveSource _waveSource;

        public event EventHandler<PlaybackStoppedEventArgs> PlaybackStopped;

        public PlaybackState PlaybackState
        {
            get
            {
                if (_soundOut != null)
                    return _soundOut.PlaybackState;
                return PlaybackState.Stopped;
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (_waveSource != null)
                    return _waveSource.GetPosition();
                return TimeSpan.Zero;
            }
            set
            {
                if (_waveSource != null)
                    _waveSource.SetPosition(value);
            }
        }

        public TimeSpan Length
        {
            get
            {
                if (_waveSource != null)
                    return _waveSource.GetLength();
                return TimeSpan.Zero;
            }
        }

        public int Volume
        {
            get
            {
                if (_soundOut != null)
                {
                    try
                    {
                        return Math.Min(100, Math.Max((int)(_soundOut.Volume * 100), 0));
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                return 100;
            }
            set
            {
                if (_soundOut != null)
                {
                    try
                    {
                        _soundOut.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }

        public void Open(string filename, MMDevice device)
        {
            CleanupPlayback();

            _waveSource =
                CodecFactory.Instance.GetCodec(filename)
                    .ToSampleSource()
                    .ToMono()
                    .ToWaveSource();
            _soundOut = new WasapiOut() { Latency = 100, Device = device };
            _soundOut.Initialize(_waveSource);
            if (PlaybackStopped != null) _soundOut.Stopped += PlaybackStopped;
        }

        public void Play()
        {
            if (_soundOut != null)
            {
                try
                {
                    _soundOut.Play();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public void Pause()
        {
            if (_soundOut != null)
            {
                try
                {
                    _soundOut.Pause();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public void Stop()
        {
            if (_soundOut != null)
            {
                try
                {
                    _soundOut.Stop();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }                
        }

        private void CleanupPlayback()
        {
            if (_soundOut != null)
            {
                _soundOut.Dispose();
                _soundOut = null;
            }
            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            CleanupPlayback();
        }
    }
}