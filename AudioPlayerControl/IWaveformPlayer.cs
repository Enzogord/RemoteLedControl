﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer.TimeLine
{
    public interface IWaveformPlayer : ISoundPlayer
    {
        /// <summary>
        /// Gets or sets the current sound streams playback position.
        /// </summary>
        double ChannelPosition { get; }

        /// <summary>
        /// Gets or sets the current sound streams playback position.
        /// </summary>
        void ChangePosition(double position);

        /// <summary>
        /// Gets the total channel length in seconds.
        /// </summary>
        double ChannelLength { get; }

        /// <summary>
        /// Gets the raw level data for the waveform.
        /// </summary>
        /// <remarks>
        /// Level data should be structured in an array where each sucessive index
        /// alternates between left or right channel data, starting with left. Index 0
        /// should be the first left level, index 1 should be the first right level, index
        /// 2 should be the second left level, etc.
        /// </remarks>
        float[] WaveformData { get; }
    }
}
