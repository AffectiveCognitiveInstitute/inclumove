using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Aci.Unity.Audio
{
    /// <summary>
    /// Distinct audio channels audio clips can be played on.
    /// </summary>
    public enum AudioChannels : uint
    {
        Master = 0,
        Assistant = 1,
        Chat = 2,
        SFX = 3
    }

    [Serializable]
    public class AudioChannelMixerDictionary : SerializableDictionary<AudioChannels, AudioMixerGroup> { }

    [Serializable]
    public class AudioChannelCapacityDictionary : SerializableDictionary<AudioChannels, uint> { }

    /// <summary>
    /// Plays audio on arget audio channel. Manages maximum amount of simultaneous audio sources.
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// <see cref="AudioMixerGroup"/> instances adressable by the service.
        /// </summary>
        IReadOnlyList<AudioMixerGroup> audioChannels { get; }

        /// <summary>
        /// Sets the target volume on a channel.
        /// </summary>
        /// <param name="volume">volume level, float value from 0 to 1.</param>
        /// <param name="channel">Target audio channel to set the volume of.</param>
        void SetVolume(float volume, AudioChannels channel);

        /// <summary>
        /// Plays an audio clip on target channel.
        /// </summary>
        /// <param name="clip">Target audio clip.</param>
        /// <param name="channel">Target channel.</param>
        void PlayAudioClip(AudioClip clip, AudioChannels channel);

        /// <summary>
        /// Plays an audio clip on target channel. Awaitable.
        /// </summary>
        /// <param name="clip">Target audio clip.</param>
        /// <param name="channel">Target channel.</param>
        /// <returns>Task instance.</returns>
        Task PlayAudioClipAsync(AudioClip clip, AudioChannels channel);
    }
}
