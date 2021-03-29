using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface
{
    public class UINumberScroller : UIBehaviour, ILayoutElement
    {
        [Serializable]
        public class TargetValueReachedEvent : UnityEvent<UINumberScroller> { }

        [Serializable]
        public struct ScrollingNumber
        {
            public RectTransform transform;
            [FormerlySerializedAs("text")]
            public TMPro.TextMeshProUGUI label;
        }

        private enum Mode
        {
            Add,
            Subtract
        }

        [SerializeField, FormerlySerializedAs("m_SpeedPerStepOverTime")]
        private AnimationCurve m_SpeedMultiplierOverTime = AnimationCurve.Linear(0, 1, 1, 1);

        [SerializeField]
        private int m_StartingValue = 0;

        [SerializeField]
        private float m_DurationPerScroll = 0.3f;

        [SerializeField]
        private ScrollingNumber m_First;

        [SerializeField]
        private ScrollingNumber m_Second;

        [SerializeField]
        private TargetValueReachedEvent m_TargetReached;

        private bool m_IsDirty = false;
        private int m_TargetValue;

        private int m_CurrentValue = 1;
        private float m_Progress;
        private float m_CurrentStepTime;
        private Mode m_Mode;
        private int m_TotalSteps;
        private int m_CurrentStep;
        private float m_OverallProgress;
        private RectTransform m_RectTransform;

        /// <summary>
        ///     The starting value.
        /// </summary>
        public int startingValue
        {
            get => m_StartingValue;
            set => m_StartingValue = value;
        }

        public RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null)
                    m_RectTransform = GetComponent<RectTransform>();

                return m_RectTransform;
            }
        }

        /// <summary>
        ///     The time it takes to complete a single scroll.
        /// </summary>
        public float durationPerScroll
        {
            get => m_DurationPerScroll;
            set => m_DurationPerScroll = value;
        }

        /// <summary>
        ///     Called when the target value has been reached
        /// </summary>
        public TargetValueReachedEvent targetReached
        {
            get
            {
                if (m_TargetReached == null)
                    m_TargetReached = new TargetValueReachedEvent();

                return m_TargetReached;
            }
        }

        public bool isTargetReached => !m_IsDirty;

        /// <summary>
        ///     The current value. 
        /// </summary>
        public int value
        {
            get => m_CurrentValue;
            set => SetValueWithoutAnimation(value);
        }


        /// <summary>
        ///     The value the UINumberScroller should reach.
        /// </summary>
        public int targetValue
        {
            get => m_TargetValue;
            set
            {
                if (m_TargetValue == value)
                    return;

                m_TargetValue = value;


                float normalizedProgress = m_CurrentStepTime / m_DurationPerScroll;

                m_CurrentStep = 0;
                m_TotalSteps = Math.Max(1, Math.Abs(m_TargetValue - m_CurrentValue));

                if (m_TargetValue > m_CurrentValue)
                    m_Mode = Mode.Add;
                else if(m_TargetValue < m_CurrentValue)
                    m_Mode = Mode.Subtract;
                else
                {
                    // if target value is current value, invert
                    if(m_Mode == Mode.Add && normalizedProgress > 0f)
                    {
                        m_CurrentValue++; 
                        m_Mode = Mode.Subtract;
                        m_CurrentStepTime = m_DurationPerScroll - m_CurrentStepTime;
                    }
                    else if (m_Mode == Mode.Subtract && normalizedProgress > 0f)
                    {
                        m_CurrentValue--;
                        m_Mode = Mode.Add;
                        m_CurrentStepTime = m_DurationPerScroll - m_CurrentStepTime;
                    }
                }

                if (normalizedProgress <= 0f)
                {
                    UpdateFirstText();
                    UpdateSecondText();
                    SetDirty();
                }

                m_IsDirty = true;
            }
        }


        public float minWidth { get; private set; }

        public float preferredWidth { get; private set; }

        public float flexibleWidth { get; private set; }

        public float minHeight { get; private set; }

        public float preferredHeight { get; private set; }

        public float flexibleHeight { get; private set; }

        public int layoutPriority { get; private set; }


        private void UpdateSecondText()
        {
            if (m_Mode == Mode.Add)
                m_Second.label.text = m_CurrentValue % 2 == 0 ? (m_CurrentValue + 1).ToString() : m_CurrentValue.ToString();
            else
                m_Second.label.text = m_CurrentValue % 2 == 0 ? (m_CurrentValue - 1).ToString() : m_CurrentValue.ToString();
        }

        private void UpdateFirstText()
        {
            if (m_Mode == Mode.Add)
                m_First.label.text = m_CurrentValue % 2 == 0 ? m_CurrentValue.ToString() : (m_CurrentValue + 1).ToString();
            else
                m_First.label.text = m_CurrentValue % 2 == 0 ? m_CurrentValue.ToString() : (m_CurrentValue - 1).ToString();
        }

        protected override void Awake()
        {
            base.Awake();
            m_CurrentValue = m_StartingValue;
            m_TargetValue = m_StartingValue;
            UpdateFirstText();
            UpdateSecondText();
            SetDirty();
            SetPosition(m_First.transform, m_CurrentValue % 2 == 0? Vector2.zero : new Vector2(0, -1),
                                           m_CurrentValue % 2 == 0? Vector2.one : new Vector2(1,0), Vector2.zero);

            SetPosition(m_Second.transform, m_CurrentValue % 2 == 0? new Vector2(0, -1) : Vector2.zero,
                                            m_CurrentValue % 2 == 0? new Vector2(1, 0) : Vector2.one, Vector2.zero);
        }



        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        private void SetPosition(RectTransform transform, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition)
        {
            transform.anchorMin = anchorMin;
            transform.anchorMax = anchorMax;
            transform.anchoredPosition = anchoredPosition;
        }

        private void Update()
        {
            if (!m_IsDirty)
                return;

            m_OverallProgress = ((float) m_CurrentStep / m_TotalSteps) +  Mathf.Min(1 ,m_CurrentStepTime / m_DurationPerScroll) * 1/m_TotalSteps;
            m_CurrentStepTime += Time.deltaTime * m_SpeedMultiplierOverTime.Evaluate(m_OverallProgress);

            // If we've exceeded one step, update current value
            if (m_CurrentStepTime > m_DurationPerScroll)
            {
                if (m_CurrentValue != m_TargetValue)
                {
                    if (m_Mode == Mode.Add)
                        m_CurrentValue++;
                    else
                        m_CurrentValue--;
                }

                m_CurrentStepTime -= m_DurationPerScroll;
                m_CurrentStep++;

                if (m_CurrentValue == m_TargetValue)
                {
                    m_CurrentStep = m_TotalSteps;
                    m_IsDirty = false;
                    m_CurrentStepTime = 0f;
                    targetReached?.Invoke(this);
                }

                UpdateFirstText();
                UpdateSecondText();
                SetDirty();
            }

            float progress = Mathf.Clamp01(m_CurrentStepTime / m_DurationPerScroll);

            UpdatePosition(progress);
        }

        private void UpdatePosition(float progress)
        {
            if (m_Mode == Mode.Add)
            {
                // First:
                // - Even: center -> up
                // - Odd: bottom -> center
                SetPosition(m_First.transform, m_CurrentValue % 2 == 0 ? Vector2.Lerp(Vector2.zero, new Vector2(0, 1), progress) : Vector2.Lerp(new Vector2(0, -1), Vector2.zero, progress),
                                               m_CurrentValue % 2 == 0 ? Vector2.Lerp(Vector2.one, new Vector2(1, 2), progress) : Vector2.Lerp(new Vector2(1, 0), Vector2.one, progress),
                                               Vector2.zero);


                // Second:
                // - Even: bottom -> center
                // - Odd: center -> up
                SetPosition(m_Second.transform, m_CurrentValue % 2 == 0 ? Vector2.Lerp(new Vector2(0, -1), Vector2.zero, progress) : Vector2.Lerp(Vector2.zero, new Vector2(0, 1), progress),
                                               m_CurrentValue % 2 == 0 ? Vector2.Lerp(new Vector2(1, 0), Vector2.one, progress) : Vector2.Lerp(Vector2.one, new Vector2(1, 2), progress),
                                               Vector2.zero);
            }
            else if (m_Mode == Mode.Subtract)
            {
                // First:
                // - Even: center -> bottom
                // - Odd: center -> top
                SetPosition(m_First.transform, m_CurrentValue % 2 == 0 ? Vector2.Lerp(Vector2.zero, new Vector2(0, -1), progress) : Vector2.Lerp(new Vector2(0, 1), Vector2.zero, progress),
                                               m_CurrentValue % 2 == 0 ? Vector2.Lerp(Vector2.one, new Vector2(1, 0), progress) : Vector2.Lerp(new Vector2(1, 2), Vector2.one, progress),
                                               Vector2.zero);

                // Second:
                // - Even: top -> center
                // - Odd: center -> bottom
                SetPosition(m_Second.transform, m_CurrentValue % 2 == 0 ? Vector2.Lerp(new Vector2(0, 1), Vector2.zero, progress) : Vector2.Lerp(Vector2.zero, new Vector2(0, -1), progress),
                                               m_CurrentValue % 2 == 0 ? Vector2.Lerp(new Vector2(1, 2), Vector2.one, progress) : Vector2.Lerp(Vector2.one, new Vector2(1, 0), progress),
                                               Vector2.zero);
            }
        }

        private void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SetDirty();
        }
#endif

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            SetDirty();
        }

        private void OnTransformChildrenChanged()
        {
            SetDirty();
        }

        public void CalculateLayoutInputHorizontal()
        {
            minWidth = preferredWidth = Mathf.Max(LayoutUtility.GetPreferredSize(m_First.transform, 0), LayoutUtility.GetPreferredSize(m_Second.transform, 0));
        }

        public void CalculateLayoutInputVertical()
        {
            minHeight = preferredHeight = Mathf.Max(LayoutUtility.GetPreferredSize(m_First.transform, 1), LayoutUtility.GetPreferredSize(m_Second.transform, 1));
        }

        public void SetValueWithoutAnimation(int value)
        {
            m_IsDirty = false;
            m_CurrentStepTime = 0f;
            m_CurrentStep = 0;
            m_CurrentValue = value;
            m_TargetValue = value;

            UpdateFirstText();
            UpdateSecondText();
            SetDirty();

            SetPosition(m_First.transform, m_CurrentValue % 2 == 0 ? Vector2.zero : new Vector2(0, -1),
                                           m_CurrentValue % 2 == 0 ? Vector2.one : new Vector2(1, 0), Vector2.zero);

            SetPosition(m_Second.transform, m_CurrentValue % 2 == 0 ? new Vector2(0, -1) : Vector2.zero,
                                            m_CurrentValue % 2 == 0 ? new Vector2(1, 0) : Vector2.one, Vector2.zero);
        }
    }
}