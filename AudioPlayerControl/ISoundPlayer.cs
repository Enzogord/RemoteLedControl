using System.ComponentModel;

namespace AudioPlayer.TimeLine
{
    public interface ISoundPlayer : INotifyPropertyChanged
    {
        bool IsEnabled { get; }

        /// <summary>
        /// Gets whether the sound player is currently playing audio.
        /// </summary>
        bool IsPlaying { get; }
    }
}
