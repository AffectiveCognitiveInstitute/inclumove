using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aci.Unity.UserInterface;
using Aci.Unity.Util;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Gamification
{
    public class AchievementManager : MonoBehaviour
    {
        [Inject]
        private DiContainer _installer;

        [Inject]
        private ITimeProvider _timeProvider;

        public AciAchievement[] achievements;

        public List<AchievementBadge> EvaluateAchievements()
        {
            List<AchievementBadge> badges = new List<AchievementBadge>();
            foreach (AciAchievement achievment in achievements)
            {
                badges.Add(achievment.Evaluate(_installer, _timeProvider));
            }

            return badges;
        }
    }
}
