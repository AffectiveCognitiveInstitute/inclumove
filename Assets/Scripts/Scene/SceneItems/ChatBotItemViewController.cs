// <copyright file=ChatBotItemViewController.cs/>
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

using Aci.Unity.Audio;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Logging;
using Aci.Unity.Util;
using BotConnector.Unity;
using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Aci.Unity.Scene.SceneItems
{
    public class ChatBotItemViewController : MonoBehaviour, IPayloadViewController
    {
        private string m_Payload;
        private float m_Delay;
        private ITimeProvider m_TimeProvider;
        private IAudioService m_AudioService;
        private IConfigProvider m_ConfigProvider;
        private ItemMode m_ItemMode;
        private ActivityData.Factory m_DataFactory;
        private IBot m_Bot;
        private ActivityFactory m_ActivityFactory;
        private string m_SpeechFilePath;

        [ConfigValue("workflowDirectory")]
        private string workflowDirectory { get; set; } = "";

        public PayloadType payloadType => PayloadType.ChatBot;

        public string payload => m_Payload;

        public float delay { get => m_Delay; set => m_Delay = value; }

        private WaitForSeconds m_WaitForSeconds;
        private AudioClip m_AudioSpeechClip;
        private bool m_SendMessage;
        private ActivityData m_ActivityData;

        private const float _epsilon = 0.25f;

        [Zenject.Inject]
        private void Construct(ItemMode itemMode,
                               IAudioService audioService,
                               IConfigProvider configProvider,
                               [Zenject.InjectOptional] ITimeProvider timeProvider,
                               [Zenject.InjectOptional] ActivityData.Factory dataFactory,
                               [Zenject.InjectOptional] IBot bot,
                               [Zenject.InjectOptional] ActivityFactory activityFactory)
        {
            m_AudioService = audioService;
            m_TimeProvider = timeProvider;
            m_ConfigProvider = configProvider;
            m_ItemMode = itemMode;
            m_DataFactory = dataFactory;
            m_Bot = bot;
            m_ActivityFactory = activityFactory;
        }

        private void Awake()
        {
            m_ConfigProvider.RegisterClient(this);
            if (m_ItemMode != ItemMode.Workflow)
                return;

            GetComponent<SpriteRenderer>().enabled = false;
        }

        private void OnDestroy()
        {
            m_ConfigProvider.UnregisterClient(this);
        }

        private void OnEnable()
        {
            if (!m_SendMessage)
                return;

            m_ActivityData = m_DataFactory.Create(payload);
            m_SpeechFilePath = Path.Combine(workflowDirectory, m_ActivityData.speechFilePath);
            StartCoroutine(SendMessageFromBot());
        }

        public void SetPayload(PayloadType type, string payload, float delay)
        {
            if (type != PayloadType.ChatBot)
                return;

            m_Payload = payload;
            m_Delay = delay;

            if (m_ItemMode == ItemMode.Editor)
                return;

            m_SendMessage = true;
        }

        private IEnumerator SendMessageFromBot()
        {
            yield return StartCoroutine(GetAudioClip(m_SpeechFilePath));
            float time = m_Delay - (float)m_TimeProvider.elapsed.TotalSeconds;
            if (time + _epsilon < 0)
                yield break;
            m_WaitForSeconds = new WaitForSeconds(time);
            yield return m_WaitForSeconds;

            m_Bot.SimulateMessageReceived(m_ActivityFactory.Create(m_ActivityData));
            if(m_AudioSpeechClip != null)
                m_AudioService.PlayAudioClip(m_AudioSpeechClip, AudioChannels.Assistant);
        }

        private IEnumerator GetAudioClip(string audioTrackUrl)
        {
            if (string.IsNullOrWhiteSpace(audioTrackUrl))
                yield break;

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"file:///{audioTrackUrl}", AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError)
                {
                    AciLog.LogError(nameof(ChatBotItemViewController), www.error);
                }
                else
                {
                    try
                    {
                        m_AudioSpeechClip = DownloadHandlerAudioClip.GetContent(www);
                    }
                    catch(Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
    }
}