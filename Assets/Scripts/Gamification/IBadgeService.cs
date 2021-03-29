using System.Collections.Generic;
using Aci.Unity.Data;

namespace Aci.Unity.Gamification
{
    public interface IBadgeService
    {
        /// <summary>
        ///     Contains the current number of badges of each type for each tier.
        /// </summary>
        BadgeData currentBadges { get; }

        /// <summary>
        ///     Contains the required number of 'fast' badges for each tier.
        ///     Elements are sorted by tier level, i.e. first index is tier 1.
        /// </summary>
        IReadOnlyList<int> fastLevels { get; }

        /// <summary>
        ///     Contains the required number of 'streak' badges for each tier.
        ///     Elements are sorted by tier level, i.e. first index is tier 1.
        /// </summary>
        IReadOnlyList<int> streakLevels { get; }

        /// <summary>
        ///     Contains the required number of 'amount' badges for each tier.
        ///     Elements are sorted by tier level, i.e. first index is tier 1.
        /// </summary>
        IReadOnlyList<int> amountLevels { get; }

        /// <summary>
        ///     The current number of 'fast' badges.
        /// </summary>
        int currentFast { get; }

        /// <summary>
        ///     The current number of 'streak' badges.
        /// </summary>
        int currentStreak { get; }

        /// <summary>
        ///     The current number of 'amount' badges.
        /// </summary>
        int currentAmount { get; }
    }
}