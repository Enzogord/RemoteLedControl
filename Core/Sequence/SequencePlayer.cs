using System;
using Core.RemoteOperations;
using NAudioPlayer;
using NLog;

namespace Core
{
    public class SequencePlayer : Player, ISequenceController, ISequenceTimeProvider
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        private SequenceState state;
        public SequenceState State {
            get => state;
            set => SetField(ref state, value, () => State);
        }

        public override void Play()
        {
            try {
                base.Play();
                State = SequenceState.Playing;
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка во время включения воспроизведения");
            }
        }

        public override void Stop()
        {
            try {
                base.Stop();
                State = SequenceState.Stoped;
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка во время остановки воспроизведения");
            }
        }

        public override void Pause()
        {
            try {
                base.Pause();
                State = SequenceState.Paused;
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка во время постановки на паузу");
            }
        }

        public double Position => ChannelPosition;

        private double lastUpdatedPosition;
        public override double ChannelPosition {
            get { return base.ChannelPosition; }
            set {
                base.ChannelPosition = value;
                OnPropertyChanged(() => Position);
                if(Math.Abs(lastUpdatedPosition - ChannelPosition) > 1D) {
                    lastUpdatedPosition = ChannelPosition;
                    currentTime = TimeSpan.FromSeconds(ChannelPosition);
                    OnPropertyChanged(() => CurrentTime);
                }
            }
        }

        private TimeSpan currentTime;
        public TimeSpan CurrentTime {
            get => currentTime;
            set {
                if(SetField(ref currentTime, value, () => CurrentTime)) {
                    ChannelPosition = currentTime.TotalSeconds;
                }
            }
        }

        public double Length => ChannelLength;

        public SequencePlayer()
        {
            PropertyChanged += SequencePlayer_PropertyChanged;
        }

        private void SequencePlayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName) {
                case nameof(IsInitialized):
                    if(!IsInitialized) {
                        State = SequenceState.NotInitialized;
                    }
                    break;
            }
        }
    }
}
