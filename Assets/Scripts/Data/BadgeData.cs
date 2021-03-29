using System;
using Unity.Mathematics;
using UnityEngine;

namespace Aci.Unity.Data
{
    [System.Serializable]
    public struct BadgeData : IEquatable<BadgeData>
    {
        public int3 AmountBadges;
        public int3 TimeBadges;
        public int3 StreakBadges;

        public static BadgeData Empty = new BadgeData()
        {
            AmountBadges = int3.zero,
            StreakBadges = int3.zero,
            TimeBadges = int3.zero
        };

        public static BadgeData operator- (BadgeData lhs, BadgeData rhs)
        {
            BadgeData val = BadgeData.Empty;
            val.AmountBadges = lhs.AmountBadges - rhs.AmountBadges;
            val.TimeBadges   = lhs.TimeBadges   - rhs.TimeBadges;
            val.StreakBadges = lhs.StreakBadges - rhs.StreakBadges;
            return val;
        }

        public static bool operator ==(BadgeData lhs, BadgeData rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(BadgeData lhs, BadgeData rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override int GetHashCode()
        {
            return AmountBadges.GetHashCode() ^ StreakBadges.GetHashCode() ^ TimeBadges.GetHashCode();
        }

        public bool Equals(BadgeData other)
        {
            bool val = true;
            val &= math.all(AmountBadges == other.AmountBadges);
            val &= math.all(TimeBadges   == other.TimeBadges);
            val &= math.all(StreakBadges == other.StreakBadges);
            return val;
        }
    }

    public static class BadgeDataExtensions
    {
        private const int TierOneWeight = 1;
        private const int TierTwoWeight = 2;
        private const int TierThreeWeight = 5;

        /// <summary>
        ///     Gets the weighted total number of badges.
        /// </summary>
        /// <param name="data">The BadgeData.</param>
        /// <returns>Returns the weighted total number of badges.</returns>
        public static int GetWeightedTotalCount(this BadgeData data)
        {
            // fast
            int fastCount = data.TimeBadges[0] * TierOneWeight + data.TimeBadges[1] * TierTwoWeight + data.TimeBadges[2] * TierThreeWeight;
            // streak
            int streakCount = data.StreakBadges[0] * TierOneWeight + data.StreakBadges[1] * TierTwoWeight + data.StreakBadges[2] * TierThreeWeight;
            // amount
            int amountCount = data.AmountBadges[0] * TierOneWeight + data.AmountBadges[1] * TierTwoWeight + data.AmountBadges[2] * TierThreeWeight;

            return fastCount + streakCount + amountCount;            
        }

        /// <summary>
        ///     Gets the total number of badges for a specific tier level.
        /// </summary>
        /// <param name="data">The BadgeData.</param>
        /// <param name="tierLevel">A value between 1 - 3.</param>
        /// <returns>Returns the total count number badges for a specific tier level.</returns>
        public static int GetTotalCount(this BadgeData data, int tierLevel)
        {
            if (tierLevel < 1 || tierLevel > 3)
                throw new ArgumentException($"{nameof(tierLevel)} must be a value between 1 - 3", nameof(data));

            int index = tierLevel - 1;
            return data.AmountBadges[index] + data.StreakBadges[index] + data.TimeBadges[index];
        }

        /// <summary>
        ///     Gets the weighted total number of 'amount' badges.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Returns the total number of 'amount' badges.</returns>
        public static int GetWeightedAmountTotalCount(this BadgeData data)
        {
            return data.AmountBadges[0] * TierOneWeight + data.AmountBadges[1] * TierTwoWeight + data.AmountBadges[2] * TierThreeWeight;
        }

        /// <summary>
        ///     Gets the weighted total number of 'streak' badges.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Returns the total number of 'streak' badges.</returns>
        public static int GetWeightedStreakTotalCount(this BadgeData data)
        {
            return data.StreakBadges[0] * TierOneWeight + data.StreakBadges[1] * TierTwoWeight + data.StreakBadges[2] * TierThreeWeight;
        }

        /// <summary>
        ///     Gets the weighted total number of 'time' badges.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Returns the total number of 'time' badges.</returns>
        public static int GetWeightedTimeTotalCount(this BadgeData data)
        {
            return data.TimeBadges[0] * TierOneWeight + data.TimeBadges[1] * TierTwoWeight + data.TimeBadges[2] * TierThreeWeight;
        }
    }
}
