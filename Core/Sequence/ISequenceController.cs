using System.ComponentModel;

namespace Core
{
    public interface ISequenceController : INotifyPropertyChanged
    {
        SequenceState State { get; }
        bool CanPlay { get; }
        bool CanPause { get; }
        bool CanStop { get; }
        void Play();
        void Pause();
        void Stop();
        double Position { get; }
        double Length { get; }
    }

    public enum SequenceState
    {
        NotInitialized,
        Stoped,
        Playing,
        Paused
    }
}
