using Aci.UI.Binding;
using Aci.Unity.Gamification;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class CurrentBadgesViewController : MonoBindable
    {
        [SerializeField, Tooltip("The badge category type to be fetched")]
        private BadgeCategory m_Category;

        private int m_TotalCount;
        private int m_Tier1Count;
        private int m_Tier2Count;
        private int m_Tier3Count;
        private IBadgeService m_BadgeService;

        /// <summary>
        ///     The total amount of badges.
        /// </summary>
        public int totalCount
        {
            get => m_TotalCount;
            set => SetProperty(ref m_TotalCount, value);
        }

        /// <summary>
        ///     The total amount of tier 1 badges.
        /// </summary>
        public int tier1Count
        {
            get => m_Tier1Count;
            set => SetProperty(ref m_Tier1Count, value);
        }

        /// <summary>
        ///     The total amount of tier 2 badges.
        /// </summary>
        public int tier2Count
        {
            get => m_Tier2Count;
            set => SetProperty(ref m_Tier2Count, value);
        }

        /// <summary>
        ///     The total amount of tier 3 badges.
        /// </summary>
        public int tier3Count
        {
            get => m_Tier3Count;
            set => SetProperty(ref m_Tier3Count, value);
        }

        [Zenject.Inject]
        private void Construct(IBadgeService badgeService)
        {
            m_BadgeService = badgeService;

            int3 tierBadges;
            switch (m_Category)
            {
                case BadgeCategory.Amount:
                    tierBadges = m_BadgeService.currentBadges.AmountBadges;
                    break;
                case BadgeCategory.Speed:
                    tierBadges = m_BadgeService.currentBadges.TimeBadges;
                    break;
                case BadgeCategory.Streak:
                    tierBadges = m_BadgeService.currentBadges.StreakBadges;
                    break;
                default:
                    return;
            }

            Set(tierBadges[0] + tierBadges[1] * 2 + tierBadges[2] * 5, tierBadges);
        }

        private void Set(int totalCount, int3 tierBadges)
        {
            this.totalCount = totalCount;
            tier1Count = tierBadges[0];
            tier2Count = tierBadges[1];
            tier3Count = tierBadges[2];
        }
    }
}