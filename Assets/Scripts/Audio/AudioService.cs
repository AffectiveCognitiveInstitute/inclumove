// <copyright file=AudioManager.cs/>
// <copyright>
//   Copyright (c) 2018, Affective & Cognitive Institute
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software andassociated documentation files
//   (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
//   merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//   LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
//   IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// <license>MIT License</license>
// <main contributors>
//   Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>07/12/2018 05:59</date>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Aci.Unity.Audio
{
    /// <summary>
    /// Plays audio clips. Preinitializes & manages audio sources.
    /// </summary>
    public class AudioService : IAudioService
    {
        private AudioServiceSettings m_Settings;

        private AudioMixerGroup[] m_MixerGroups;
        private uint[] m_MixerGroupCapacities;
        private List<AudioSource>[] m_ClosedSources;
        private List<CancellationTokenSource>[] m_ClosedTokens;
        private Queue<AudioSource> m_OpenSources = new Queue<AudioSource>();

        /// <inheritdoc/>
        public IReadOnlyList<AudioMixerGroup> audioChannels => m_MixerGroups;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mixer"></param>
        public AudioService(AudioServiceSettings settings)
        {
            m_Settings = settings;

            m_MixerGroups = new AudioMixerGroup[m_Settings.channels.Count];
            m_MixerGroupCapacities = new uint[m_Settings.channels.Count];
            m_ClosedSources = new List<AudioSource>[m_Settings.channels.Count];
            m_ClosedTokens = new List<CancellationTokenSource>[m_Settings.channels.Count];

            uint totalCapacity = 0;
            for(uint i = 0; i <= (uint)AudioChannels.SFX; ++i)
            {
                m_MixerGroups[i] = m_Settings.channels[(AudioChannels)i];
                m_MixerGroupCapacities[i] = m_Settings.capacities[(AudioChannels)i];
                m_ClosedSources[i] = new List<AudioSource>((int)(m_Settings.capacities[(AudioChannels)i]));
                m_ClosedTokens[i] = new List<CancellationTokenSource>((int)m_Settings.capacities[(AudioChannels)i]);
                totalCapacity += m_Settings.capacities[(AudioChannels)i];
            }

            AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(5000f, 1f));
            for (uint i = 0; i < totalCapacity; ++i)
            {
                GameObject gObj = new GameObject();
                gObj.name = "Pooled_AudioSource_" + i.ToString();
                AudioSource src = gObj.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.rolloffMode = AudioRolloffMode.Custom;
                src.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
                m_OpenSources.Enqueue(src);
            }
        }
        
        private async Task WaitForEnd(AudioClip clip, float length, AudioChannels channel, CancellationToken token)
        {
            // waited for specified time
            try
            {
                await Task.Delay((int) (length * 1000), token);
            }
            catch(TaskCanceledException e)
            {
                if (token.IsCancellationRequested)
                    return;
            }

            m_ClosedSources[(uint)channel][0].Stop();
            m_OpenSources.Enqueue(m_ClosedSources[(uint)channel][0]);
            m_ClosedSources[(uint)channel].RemoveAt(0);
            m_ClosedTokens[(uint)channel].RemoveAt(0);
        }

        /// <inheritdoc/>
        public void SetVolume(float volume, AudioChannels channel)
        {
            throw new System.NotImplementedException();
        }

        private CancellationToken PlayClip_Internal(AudioClip clip, AudioChannels channel)
        {
            // if audio channel is full remove newest listener
            if (m_ClosedSources[(uint)channel].Count >= m_MixerGroupCapacities[(uint)channel])
            {
                m_ClosedSources[(uint)channel][0].Stop();
                m_OpenSources.Enqueue(m_ClosedSources[(uint)channel][0]);
                m_ClosedSources[(uint)channel].RemoveAt(0);
                m_ClosedTokens[(uint)channel][0].Cancel();
                m_ClosedTokens[(uint)channel].RemoveAt(0);
            }

            //add source play and start wait task
            AudioSource source = m_OpenSources.Dequeue();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            source.clip = clip;
            source.outputAudioMixerGroup = m_MixerGroups[(uint)channel];
            source.Play();
            m_ClosedSources[(uint)channel].Add(source);
            m_ClosedTokens[(uint)channel].Add(tokenSource);
            return tokenSource.Token;
        }

        /// <inheritdoc/>
        public void PlayAudioClip(AudioClip clip, AudioChannels channel)
        {
            CancellationToken token = PlayClip_Internal(clip, channel);

            WaitForEnd(clip, clip.length, channel, token);
        }

        /// <inheritdoc/>
        public async Task PlayAudioClipAsync(AudioClip clip, AudioChannels channel)
        {
            CancellationToken token = PlayClip_Internal(clip, channel);

            await WaitForEnd(clip, clip.length, channel, token);
        }
    }
}