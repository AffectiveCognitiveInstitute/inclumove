using System;
using Aci.Unity.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Aci.Unity.UserInterface
{
    /// <summary>
    ///     Component which plays an audio clip either on the Pointer Down event or Pointer Up event
    /// </summary>
    [RequireComponent(typeof(AudioEventSender), typeof(Button))]
    public class PlayAudioOnClick: MonoBehaviour, IPointerDownHandler, IPointerClickHandler
    {
        [SerializeField, Tooltip("The audio clip that should be played")]
        private AudioClip m_AudioClip;

        [SerializeField, Tooltip("The audio channel the clip should be played on")]
        private AudioChannels m_AudioChannel;

        [SerializeField, Tooltip("Should the audio clip be played when the pointer is down or is up?")]
        private bool m_PlayAudioOnPointerUp = false;

        private IAudioService m_AudioService;
        private Button m_Button;

        private void Awake()
        {
            m_Button = GetComponent<Button>();
        }

        [Inject]
        private void Construct(IAudioService audioService)
        {
            m_AudioService = audioService;
        }

        private void Play()
        {
            if (!m_Button.enabled)
                return;
            try
            {
                m_AudioService.PlayAudioClip(m_AudioClip, m_AudioChannel);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!m_PlayAudioOnPointerUp)
                return;

            Play();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_PlayAudioOnPointerUp)
                return;

            Play();
        }
    }
}