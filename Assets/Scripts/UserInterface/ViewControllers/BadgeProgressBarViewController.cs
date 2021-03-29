using Aci.UI.Binding;
using Aci.Unity.Gamification;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class BadgeProgressBarViewController : MonoBindable
    {
        [SerializeField]
        private BadgeCategory m_BadgeCategory;

        [SerializeField]
        private int m_TierLevel = 1;
        
        private float m_NormalizedProgress;
        private bool m_IsBadgeVisible;
        private IBadgeService m_BadgeService;

        /// <summary>
        ///     The tier level.
        /// </summary>
        public int tierLevel
        {
            get => m_TierLevel;
            set => SetProperty(ref m_TierLevel, value);
        }

        /// <summary>
        ///     Should the badge be visible on the progress bar?
        /// </summary>
        public bool isBadgeVisible
        {
            get => m_IsBadgeVisible;
            set => SetProperty(ref m_IsBadgeVisible, value);
        }

        /// <summary>
        ///     The user's progress on this tier level for the current <see cref="BadgeCategory"/>.
        /// </summary>
        public float normalizedProgress
        {
            get => m_NormalizedProgress;
            set => SetProperty(ref m_NormalizedProgress, value);
        }

        [Zenject.Inject]
        private void Construct(IBadgeService badgeService)
        {
            m_BadgeService = badgeService;
        }

        private void Awake()
        {
            IReadOnlyCollection<int> levels = GetBadgeLevels();
            int currentBadges = GetCurrentBadges();
            int min, max;
            GetMinMax(levels, out min, out max);
            normalizedProgress = Mathf.InverseLerp(min, max, currentBadges);
            isBadgeVisible = currentBadges >= min;
        }

        private IReadOnlyCollection<int> GetBadgeLevels()
        {
            switch (m_BadgeCategory)
            {
                case BadgeCategory.Amount:
                    return m_BadgeService.amountLevels;
                case BadgeCategory.Speed:
                    return m_BadgeService.fastLevels;
                case BadgeCategory.Streak:
                    return m_BadgeService.streakLevels;
            }

            return null;
        }

        private int GetCurrentBadges()
        {
            switch (m_BadgeCategory)
            {
                case BadgeCategory.Amount:
                    return m_BadgeService.currentAmount;
                case BadgeCategory.Speed:
                    return m_BadgeService.currentFast;
                case BadgeCategory.Streak:
                    return m_BadgeService.currentStreak;
            }

            return -1;
        }

        private void GetMinMax(IReadOnlyCollection<int> levels, out int min, out int max)
        {
            min = max = 0;
            switch (m_TierLevel)
            {
                case 1:
                    min = 0;
                    max = levels.ElementAt(0);
                    break;
                case 2:
                    min = m_BadgeCategory == BadgeCategory.Amount ? levels.ElementAt(0) : 0;
                    max = levels.ElementAt(1);
                    break;
                case 3:
                    min = m_BadgeCategory == BadgeCategory.Amount ? levels.ElementAt(1) : 0;
                    max = levels.ElementAt(2);
                    break;
            }
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            m_TierLevel = Mathf.Clamp(m_TierLevel, 1, 3);
        }
#endif
    }
}