using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Aci.Unity.Data;
using Aci.Unity.UserInterface.Animation;
using UnityEngine.UI.Extensions;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class BadgeOverviewViewController : MonoBehaviour, IQueueableAnimation
    {
        [SerializeField]
        private BadgeViewController m_FastBadge;

        [SerializeField]
        private BadgeViewController m_StreakBadge;

        [SerializeField]
        private BadgeViewController m_AmountBadge;

        [SerializeField]
        private QueueableTweenerDirector m_AmountAnimation;

        [SerializeField]
        private TieredGradientGroupViewController m_AmountAnimationColorizer;

        [SerializeField]
        private QueueableTweenerDirector m_FastAnimation;

        [SerializeField]
        private TieredGradientGroupViewController m_FastAnimationColorizer;

        [SerializeField]
        private StreakAnimationViewController m_StreakAnimation;

        private IAnimationQueue m_AnimationQueue = new AnimationQueue();

        private async void Start()
        {
            await Task.Delay(40);
            UIPolygon[] polys = m_AmountBadge.GetComponentsInChildren<UIPolygon>();
            foreach (UIPolygon poly in polys)
                poly.SetAllDirty();
            polys = m_FastBadge.GetComponentsInChildren<UIPolygon>();
            foreach (UIPolygon poly in polys)
                poly.SetAllDirty();
            polys = m_StreakBadge.GetComponentsInChildren<UIPolygon>();
            foreach (UIPolygon poly in polys)
                poly.SetAllDirty();
        }

        public void SetBadgeData(BadgeData data, bool applyInstantly = false)
        {
            // fast
            int fastCount = data.GetWeightedTimeTotalCount();
            m_FastBadge.SetCounterTarget(m_FastBadge.currentValue + fastCount);
            // streak
            int streakCount = data.GetWeightedStreakTotalCount();
            m_StreakBadge.SetCounterTarget(m_StreakBadge.currentValue + streakCount);
            // amount
            int amountCount = data.GetWeightedAmountTotalCount();
            m_AmountBadge.SetCounterTarget(m_AmountBadge.currentValue + amountCount);

            if (applyInstantly)
            {
                m_AmountBadge.Play();
                m_FastBadge.Play();
                m_StreakBadge.Play();
                return;
            }

            QueueableAnimationSynchronizer synchedAnim = null;
            if (amountCount > 0)
            {
                int badgeTier = data.AmountBadges[2] > 0 ? 2 : data.AmountBadges[1] > 0 ? 1 : 0;
                // fade out badges
                synchedAnim = QueueableAnimationSynchronizer.Pool.Spawn();
                m_AmountBadge.SetTier(badgeTier);
                m_AmountAnimationColorizer.SetTier((uint)badgeTier);
                synchedAnim.Append(m_AmountBadge.tierFadeInTweener, m_FastBadge.fadeOutTweener, m_StreakBadge.fadeOutTweener);
                m_AnimationQueue.Enqueue(synchedAnim);
                // badge counter +  top bar highlight animation
                synchedAnim = QueueableAnimationSynchronizer.Pool.Spawn();
                synchedAnim.Append(m_AmountBadge, m_AmountAnimation);
                m_AnimationQueue.Enqueue(synchedAnim);
                // fade int badges
                synchedAnim = QueueableAnimationSynchronizer.Pool.Spawn();
                synchedAnim.Append(m_AmountBadge.tierFadeOutTweener, m_FastBadge.fadeInTweener, m_StreakBadge.fadeInTweener);
                m_AnimationQueue.Enqueue(synchedAnim);
            }

            if (fastCount > 0)
            {
                int badgeTier = data.TimeBadges[2] > 0 ? 2 : data.TimeBadges[1] > 0 ? 1 : 0;
                // fade out badges
                synchedAnim = QueueableAnimationSynchronizer.Pool.Spawn();
                m_FastBadge.SetTier(badgeTier);
                m_FastAnimationColorizer.SetTier((uint)badgeTier);
                synchedAnim.Append(m_AmountBadge.fadeOutTweener, m_FastBadge.tierFadeInTweener, m_StreakBadge.fadeOutTweener);
                m_AnimationQueue.Enqueue(synchedAnim);
                // badge counter +  top bar highlight animation
                synchedAnim = QueueableAnimationSynchronizer.Pool.Spawn();
                synchedAnim.Append(m_FastBadge, m_FastAnimation);
                m_AnimationQueue.Enqueue(synchedAnim);
                // fade int badges
                synchedAnim = QueueableAnimationSynchronizer.Pool.Spawn();
                synchedAnim.Append(m_AmountBadge.fadeInTweener, m_FastBadge.tierFadeOutTweener, m_StreakBadge.fadeInTweener);
                m_AnimationQueue.Enqueue(synchedAnim);
            }

            if (streakCount > 0)
            {
                int badgeTier = data.StreakBadges[2] > 0 ? 2 : data.StreakBadges[1] > 0 ? 1 : 0;
                // fade out badges
                synchedAnim = QueueableAnimationSynchronizer.Pool.Spawn();
                m_StreakBadge.SetTier(badgeTier);
                synchedAnim.Append(m_AmountBadge.fadeOutTweener, m_FastBadge.fadeOutTweener, m_StreakBadge.tierFadeInTweener);
                m_AnimationQueue.Enqueue(synchedAnim);
                // badge counter +  top bar highlight animation
                m_StreakAnimation.SetBadgeTier(badgeTier);
                synchedAnim = QueueableAnimationSynchronizer.Pool.Spawn();
                synchedAnim.Append(m_StreakBadge, m_StreakAnimation);
                m_AnimationQueue.Enqueue(synchedAnim);
                // fade int badges
                synchedAnim = QueueableAnimationSynchronizer.Pool.Spawn();
                synchedAnim.Append(m_AmountBadge.fadeInTweener, m_FastBadge.fadeInTweener, m_StreakBadge.tierFadeOutTweener);
                m_AnimationQueue.Enqueue(synchedAnim);
            }
        }

        public async Task Play()
        {
            await m_AnimationQueue.Play();
            return;
        }
    }
}
