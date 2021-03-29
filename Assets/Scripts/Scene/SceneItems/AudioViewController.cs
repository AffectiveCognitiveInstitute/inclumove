// <copyright file=SceneSprite.cs/>
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

using System.IO;
using System.Threading.Tasks;
using Aci.Unity.Audio;
using Aci.Unity.Util;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Aci.Unity.Scene.SceneItems
{
    /// <summary>
    ///     SceneRect can be used to render unity asset based textures.
    /// </summary>
    [RequireComponent(typeof(AudioEventSender))]
    public class AudioViewController : MonoBehaviour, IPayloadViewController
    {
        private ITimeProvider m_TimeProvider;
        private IAudioService m_AudioService = null;
        private IConfigProvider m_ConfigProvider;
        public PayloadType payloadType => PayloadType.Audio;

        [ConfigValue("workflowDirectory")]
        private string workflowDirectory { get; set; } = "";

        private string m_Payload;
        public string payload => m_Payload;

        private float m_Delay;
        public float delay
        {
            get => m_Delay;
            set
            {
                if (m_Delay == value)
                    return;
                m_Delay = value;
            }
        }

        private AudioClip m_Clip = null;

        [Zenject.Inject]
        private void Construct(ItemMode mode,
                               IAudioService audioService,
                               IConfigProvider configProvider,
                               [Zenject.InjectOptional] ITimeProvider timeProvider)
        {
            m_TimeProvider = timeProvider;
            m_AudioService = audioService;
            m_ConfigProvider = configProvider;

            if (mode != ItemMode.Workflow)
                return;
            GetComponent<SpriteRenderer>().enabled = false;
        }

        private void Start()
        {
            m_ConfigProvider.RegisterClient(this);
        }

        private void OnDestroy()
        {
            m_ConfigProvider.UnregisterClient(this);
        }

        public void SetPayload(PayloadType type, string payload, float delay)
        {
            if (type != PayloadType.Audio)
                return;
            m_Payload = payload;
            m_Delay = delay;

            if (!File.Exists(payload))
                return;
            GetAudioClip(Path.Combine(workflowDirectory, payload));
        }

        private async void GetAudioClip(string audioTrackUrl)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"file:///{audioTrackUrl}", AudioType.WAV))
            {
                await www.SendWebRequest();

                if (www.isNetworkError)
                {
                    Aci.Unity.Logging.AciLog.LogError("SceneAudio", www.error);
                }
                else
                {
                    m_Clip = DownloadHandlerAudioClip.GetContent(www);
                    if (isActiveAndEnabled)
                        OnEnable();
                }
            }
        }

        private async void OnEnable()
        {
            if (m_Clip == null)
                return;
            float time = m_Delay - (float)(m_TimeProvider?.elapsed.TotalSeconds ?? 0f);
            if (time < 0)
                return;
            await Task.Delay(Mathf.FloorToInt(time * 1000));
            m_AudioService.PlayAudioClip(m_Clip, AudioChannels.SFX);
        }
    }
}