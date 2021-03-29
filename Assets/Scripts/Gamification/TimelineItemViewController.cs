using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aci.Unity.UI.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Aci.Unity.UserInterface;
using Gradient = UnityEngine.Gradient;
using Aci.Unity.UserInterface.Animation;

namespace Aci.Unity.Gamification
{
    class TimelineItemViewController : MonoBehaviour, ITimelineItemViewController
    {
        [SerializeField]
        private Slider m_ProgressSlider;

        [SerializeField]
        private AnchorTweener m_ActivityTweener;

        [SerializeField]
        private AlphaTweener m_ProgressTweener;

        [SerializeField]
        private QueueableTweener m_ProgressTweenerAnimation;

        [SerializeField]
        private AnchorTweener m_ParticleTweener;

        [SerializeField]
        private QueueableTweener m_ParticleTweenerAnimation;

        [SerializeField]
        private ParticleSystem m_ActivityParticles;

        [SerializeField]
        private RectTransform m_OutlineTransform;

        [SerializeField]
        private Gradient2 m_Background;

        [SerializeField]
        private Gradient2 m_ProgressFill;

        [SerializeField]
        private Gradient[] m_ProgressLevels;

        [SerializeField, Tooltip("If true updates the progress dynamically, otherwise just switches to opaque on completion.")]
        private bool m_UpdateProgress;

        private RectTransform m_Transform;

        private bool m_Active;
        /// <inheritdoc />
        public bool active
        {
            get => m_Active;
            set
            {
                if (m_Active == value)
                    return;
                m_Active = value;
                if (value)
                {
                    m_ActivityTweener.PlayForwards();
                    if(hasParticles)
                    {
                        m_ActivityParticles.Play();
                        ParticleSystem.MainModule module = m_ActivityParticles.main;
                        module.simulationSpeed = 1f;
                    }
                    else
                    {
                        ParticleSystem.MainModule module = m_ActivityParticles.main;
                        module.simulationSpeed = 0f;
                        m_ActivityParticles.Stop();
                    }
                }
                else
                {
                    m_ActivityTweener.Seek(0);
                    m_ActivityTweener.Stop();
                    ParticleSystem.MainModule module = m_ActivityParticles.main;
                    module.simulationSpeed = 0f;
                    m_ActivityParticles.Pause();
                }
            }
        }

        private bool m_Completed;
        /// <inheritdoc />
        public bool completed
        {
            get => m_Completed;
            set
            {
                if (value && m_Progress <= 1f)
                    progress = 1f;
                m_Completed = value;
            }
        }

        private float m_Progress;

        /// <inheritdoc />
        public float progress
        {
            get => m_Progress;
            set
            {
                m_Progress = Mathf.Min(value, m_ProgressLevels.Length-1);
                float curProgress = m_ProgressSlider.value;
                int progressLevel = Mathf.Clamp(Mathf.FloorToInt(value - 0.01f), 0, m_ProgressLevels.Length-2);
                m_ProgressSlider.value = m_Progress - progressLevel;
                if (m_UpdateProgress)
                {
                    m_Background.EffectGradient = m_ProgressLevels[progressLevel];
                    m_ProgressFill.EffectGradient = m_ProgressLevels[progressLevel + 1];
                }
                else
                {
                    m_Background.EffectGradient = m_ProgressLevels[value < 1 ? 0 : 1];
                    m_ProgressFill.EffectGradient = m_ProgressLevels[value < 1 ? 0 : 1];
                }
            }
        }

        /// <inheritdoc />
        public Vector2 size
        {
            get => m_Transform.anchorMax - m_Transform.anchorMin;
            set
            {
                m_Transform.anchorMax = m_Transform.anchorMin + value;
                m_Transform.offsetMin = Vector2.zero;
                m_Transform.offsetMax = Vector2.zero;
                m_Transform.localScale = Vector3.one;
                Vector2 refSize = m_Transform.rect.size;
                float aspect = refSize.y / refSize.x;
                m_OutlineTransform.anchorMin = new Vector2(0.1f * aspect, 0.1f);
                m_OutlineTransform.anchorMax = new Vector2(1f - 0.1f * aspect, 0.9f);
                m_OutlineTransform.offsetMin = Vector2.zero;
                m_OutlineTransform.offsetMax = Vector2.zero;
                m_OutlineTransform.localScale = Vector3.one;
                Vector2 offsetVector = Vector2.one * 0.05f;
                offsetVector.x = 0;
                if(m_OutlineTransform.TryGetComponent<UIShearedRectangle>(out UIShearedRectangle rectangle))
                {
                    rectangle.thickness = m_Transform.rect.height * 0.1f * 0.2f;
                }
                else if(m_OutlineTransform.TryGetComponent<UICircle>(out UICircle circle))
                {
                    circle.Thickness = m_Transform.rect.height * 0.1f * 0.2f;
                }
                m_ActivityTweener.fromValue = new RectTransformAnchor()
                {
                    min = m_OutlineTransform.anchorMin - offsetVector,
                    max = m_OutlineTransform.anchorMax - offsetVector
                };
                m_ActivityTweener.toValue = new RectTransformAnchor()
                {
                    min = m_OutlineTransform.anchorMin + offsetVector,
                    max = m_OutlineTransform.anchorMax + offsetVector
                };
            }
        }

        /// <inheritdoc />
        public Vector2 position
        {
            get => m_Transform.anchorMin;
            set => m_Transform.anchorMin = value;
        }

        public IQueueableAnimation progressCompleteAnimation => m_ParticleTweenerAnimation;

        public IQueueableAnimation progressResetAnimation => m_ProgressTweenerAnimation;

        public bool hasParticles { get; set; } = false;

        private void Awake()
        {
            m_Transform = transform as RectTransform;
        }

        public void Reset()
        {
            completed = false;
            progress = 0f;
            m_ProgressTweener.Seek(0);
            m_ActivityTweener.Seek(0);
            m_ParticleTweener.Seek(0);
        }
    }
}
