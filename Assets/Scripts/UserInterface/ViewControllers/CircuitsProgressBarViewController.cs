using Aci.UI.Binding;
using Aci.Unity.Gamification;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class CircuitsProgressBarViewController : MonoBindable
    {
        private int m_CurrentCircuitsCount;
        private int m_MaxCircuitsCount;
        private float m_CurrentNormalizedProgress;
        private int m_NextTierLevel;
        private float m_NormalizedNextLevelValue;
        private IBadgeService m_BadgeService;

        /// <summary>
        ///     The current number of circuits.
        /// </summary>
        public int currentCircuitsCount
        {
            get => m_CurrentCircuitsCount;
            set => SetProperty(ref m_CurrentCircuitsCount, value);
        }

        /// <summary>
        ///     The maximum number of circuits.
        /// </summary>
        public int maxCircuitsCount
        {
            get => m_MaxCircuitsCount;
            set => SetProperty(ref m_MaxCircuitsCount, value);
        }

        /// <summary>
        ///     The current normalized progress.
        /// </summary>
        public float currentNormalizedProgress
        {
            get => m_CurrentNormalizedProgress;
            set => SetProperty(ref m_CurrentNormalizedProgress, value);
        }

        /// <summary>
        ///     The next tier level the user can reach.
        /// </summary>
        public int nextTierLevel
        {
            get => m_NextTierLevel;
            set => SetProperty(ref m_NextTierLevel, value);
        }

        /// <summary>
        ///     The normalized value on the progress bar where the next tier can be found.
        /// </summary>
        public float normalizedNextTierLevelValue
        {
            get => m_NormalizedNextLevelValue;
            set => SetProperty(ref m_NormalizedNextLevelValue, value);
        }

        [Zenject.Inject]
        private void Construct(IBadgeService badgeService)
        {
            m_BadgeService = badgeService;
        }

        private void Awake()
        {
            currentCircuitsCount = m_BadgeService.currentAmount;
            maxCircuitsCount = m_BadgeService.amountLevels.Max();
            currentNormalizedProgress = (float) currentCircuitsCount / maxCircuitsCount;
            nextTierLevel = GetNextTierLevel();
            normalizedNextTierLevelValue = (float) m_BadgeService.amountLevels.ElementAt(nextTierLevel-1) / maxCircuitsCount;
        }

        private int GetNextTierLevel()
        {
            int current = m_BadgeService.currentAmount;
            int nextTierIndex = 1;
            foreach (int amountLevel in m_BadgeService.amountLevels)
            {
                if (current < amountLevel)
                    return nextTierIndex;

                nextTierIndex++;
            }

            return Mathf.Min(nextTierIndex, 3);
        }
    }
}