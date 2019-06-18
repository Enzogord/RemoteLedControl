using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer.TimeLine
{
    public interface ISoundPlayer : INotifyPropertyChanged
    {
        bool IsInitialized { get; }

        /// <summary>
        /// Gets whether the sound player is currently playing audio.
        /// </summary>
        bool IsPlaying { get; }
    }
}
