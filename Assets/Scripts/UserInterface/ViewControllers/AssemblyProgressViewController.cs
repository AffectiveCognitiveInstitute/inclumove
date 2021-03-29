using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Aci.Unity.UserInterface.Animation;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class AssemblyProgressViewController : MonoBehaviour, IQueueableAnimation
    {
        [SerializeField]
        private Slider m_Slider;

        [SerializeField]
        private float m_Duration = 1f;

        private float m_Start;
        private float m_End;
        private float m_PassedTime;
        private bool m_IsDirty = false;

        public void SetValue(float value)
        {
            m_Slider.value = value;
        }

        public void SetTarget(float target)
        {
            m_Start = m_Slider.value;
            m_End = target;
        }

        public async Task Play()
        {
            m_IsDirty = true;
            m_PassedTime = 0f;
            while(m_IsDirty)
                await new WaitForEndOfFrame();
        }

        private void Start()
        {
            m_Slider.value = 0f;
        }

        private void Update()
        {
            if (!m_IsDirty)
                return;
            m_PassedTime += Time.deltaTime;
            float t = Mathf.Clamp01(m_PassedTime / m_Duration);
            m_Slider.value = Mathf.Lerp(m_Start, m_End, t);
            if (t < 1)
                return;
            m_IsDirty = false;
        }
    }
}
