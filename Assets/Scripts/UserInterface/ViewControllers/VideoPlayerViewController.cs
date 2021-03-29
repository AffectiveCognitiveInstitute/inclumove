using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class VideoPlayerViewController : MonoBehaviour
    {
        [SerializeField]
        private RawImage m_RawImage;

        [SerializeField]
        private VideoPlayer m_VideoPlayer;

        [SerializeField]
        private Slider m_Slider;

        [SerializeField]
        private ToggleButton m_PlayToggle;

        [SerializeField]
        private ToggleButton m_VolumeToggle;

        [SerializeField]
        private TMPro.TextMeshProUGUI m_CurrentMinute;
        [SerializeField]
        private TMPro.TextMeshProUGUI m_CurrentSecond;
        [SerializeField]
        private TMPro.TextMeshProUGUI m_DurationMinute;
        [SerializeField]
        private TMPro.TextMeshProUGUI m_DurationSeconds;

        // Reduce GC
        private static readonly string[] s_Times = new string[]
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
            "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
            "31", "32", "33", "34", "35", "36", "37", "38", "39", "40",
            "41", "42", "43", "44", "45", "46", "47", "48", "49", "50",
            "51", "52", "53", "54", "55", "56", "57", "58", "59"
        };

        private void Awake()
        {
            m_Slider.minValue = 0;
            m_Slider.maxValue = 1;
            m_Slider.SetValueWithoutNotify(0);
            m_PlayToggle.interactable = false;
            m_VolumeToggle.interactable = false;
        }

        private void OnEnable()
        {
            m_VideoPlayer.loopPointReached += OnLoopPointReached;
            m_PlayToggle.onToggle.AddListener(OnPlayToggled);
            m_VolumeToggle.onToggle.AddListener(OnVolumeToggled);
            m_VideoPlayer.prepareCompleted += OnPrepareCompleted;
            m_Slider.onValueChanged.AddListener(OnSlideValueChanged);
        }

        private void OnDisable()
        {
            m_VideoPlayer.loopPointReached -= OnLoopPointReached;
            m_PlayToggle.onToggle.RemoveListener(OnPlayToggled);
            m_VolumeToggle.onToggle.RemoveListener(OnVolumeToggled);
            m_VideoPlayer.prepareCompleted -= OnPrepareCompleted;
            m_Slider.onValueChanged.RemoveListener(OnSlideValueChanged);
        }

        private void OnLoopPointReached(VideoPlayer source)
        {
            m_PlayToggle.isOn = false;
            m_CurrentSecond.text = s_Times[0];
            m_CurrentMinute.text = s_Times[0];
            m_Slider.SetValueWithoutNotify(0);
        }

        private void OnSlideValueChanged(float value)
        {
            m_VideoPlayer.time = Mathf.Lerp(0, (float) m_VideoPlayer.length, value);
            UpdateTime(m_VideoPlayer.time, m_CurrentMinute, m_CurrentSecond);
        }

        private void OnVolumeToggled(bool isOn)
        {
            m_VideoPlayer.SetDirectAudioVolume(0, isOn ? 1f : 0f);
        }

        private void OnPlayToggled(bool isOn)
        {
            if (!m_VideoPlayer.isPrepared)
                return;

            if (isOn)
                m_VideoPlayer.Play();
            else
            {
                if(m_VideoPlayer.isPlaying)
                    m_VideoPlayer.Pause();
            }
        }

        private void OnPrepareCompleted(VideoPlayer source)
        {
            m_RawImage.texture = m_VideoPlayer.texture;
            m_PlayToggle.interactable = true;
            m_VolumeToggle.interactable = true;
            m_Slider.interactable = true;
            m_PlayToggle.isOn = true;
            UpdateTime(m_VideoPlayer.length, m_DurationMinute, m_DurationSeconds);
            m_VideoPlayer.Play();
        }

        private void SetDirty()
        {
            m_Slider.SetValueWithoutNotify(0);
            m_PlayToggle.interactable = false;
            m_VolumeToggle.interactable = false;
            m_Slider.interactable = false;
            m_VideoPlayer.Prepare();
            m_CurrentSecond.text = s_Times[0];
            m_CurrentMinute.text = s_Times[0];
            m_DurationMinute.text = s_Times[0];
            m_DurationSeconds.text = s_Times[0];
        }

        private void UpdateTime(double time, TMPro.TextMeshProUGUI minutesView, TMPro.TextMeshProUGUI secondsView)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(Math.Max(0, time));
            minutesView.text = s_Times[timeSpan.Minutes];
            secondsView.text = s_Times[timeSpan.Seconds];
        }

        private void Update()
        {
            if (!m_VideoPlayer.isPlaying)
                return;

            m_Slider.SetValueWithoutNotify((float) m_VideoPlayer.frame / m_VideoPlayer.frameCount);
            UpdateTime(m_VideoPlayer.time, m_CurrentMinute, m_CurrentSecond);
        }

        public void SetUrl(string url)
        {
            m_VideoPlayer.source = VideoSource.Url;
            m_VideoPlayer.url = url;
            SetDirty();
        }

        public void SetClip(VideoClip videoClip)
        {
            m_VideoPlayer.source = VideoSource.VideoClip;
            m_VideoPlayer.clip = videoClip;
            SetDirty();
        }
    }
}