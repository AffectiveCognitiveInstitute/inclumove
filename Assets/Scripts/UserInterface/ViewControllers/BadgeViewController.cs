using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Aci.Unity.UserInterface.Animation;
using Aci.Unity.UI.Tweening;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class BadgeViewController : MonoBehaviour, IQueueableAnimation
    {
        [SerializeField]
        private UINumberScroller m_NumberScroller;

        [SerializeField]
        private QueueableTweener m_FadeInTweener;

        [SerializeField]
        private QueueableTweener m_FadeOutTweener;

        [SerializeField]
        private QueueableTweener m_TierFadeInTweener;

        [SerializeField]
        private QueueableTweener m_TierFadeOutTweener;

        [SerializeField]
        private Image m_TierImage;

        [SerializeField]
        private Sprite[] m_BadgeTiers;

        public IQueueableAnimation fadeInTweener => m_FadeInTweener;
        public IQueueableAnimation fadeOutTweener => m_FadeOutTweener;
        public IQueueableAnimation tierFadeInTweener => m_TierFadeInTweener;
        public IQueueableAnimation tierFadeOutTweener => m_TierFadeOutTweener;

        private int m_Target;

        public int currentValue => m_NumberScroller.value;

        private void Start()
        {
            m_NumberScroller.targetValue = 0;
        }

        public void SetCounterTarget(int target)
        {
            m_Target = target;
        }

        public void SetTier(int tier)
        {
            m_TierImage.sprite = m_BadgeTiers[tier];
        }

        public async Task Play()
        {
            m_NumberScroller.targetValue = m_Target;
            while(!m_NumberScroller.isTargetReached)
                await new WaitForEndOfFrame();
        }
    }
}
