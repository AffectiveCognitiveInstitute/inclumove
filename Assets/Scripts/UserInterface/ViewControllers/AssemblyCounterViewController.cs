using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Aci.Unity.UserInterface.Animation;
using Aci.Unity.UI.Tweening;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class AssemblyCounterViewController : MonoBehaviour, IQueueableAnimation
    {
        [SerializeField]
        private UINumberScroller m_NumberScroller;

        [SerializeField]
        private AlphaTweener m_AlphaTweener;

        [SerializeField]
        private Slider m_Slider;

        private float m_Start;
        private float m_End;
        private bool m_IsDirty = false;

        private int m_Target;

        private void Start()
        {
            m_NumberScroller.targetValue = 0;
        }

        public void SetTarget(int target)
        {
            m_Target = target;
        }

        public async Task Play()
        {
            m_NumberScroller.targetValue = m_Target;
            m_AlphaTweener.PlayForwardsAsync();
            while (!m_NumberScroller.isTargetReached && m_AlphaTweener.isPlaying)
                await new WaitForEndOfFrame();
            m_Slider.value = 0;
            m_AlphaTweener.PlayReverseAsync();
        }
    }
}
