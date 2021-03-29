using System.Collections;
using System.Collections.Generic;
using Aci.Unity.UserInterface;
using Aci.Unity.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Aci.Unity.Gamification
{
    /// <summary>
    ///     Base class for achievements. Classes implementing this should specify their evaluation method, generating the corresponding badge.
    /// </summary>
    public abstract class AciAchievement : ScriptableObject
    {
        /// <summary>
        ///     Evaluates whether achievement conditions were met and generates the corresponding badge.
        /// </summary>
        /// <returns>Corresponding acheivement badge.</returns>
        public abstract AchievementBadge Evaluate(DiContainer installer, ITimeProvider timeProvider);
    }
}
