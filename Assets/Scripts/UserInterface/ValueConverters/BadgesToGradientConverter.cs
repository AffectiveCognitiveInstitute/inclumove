using Aci.UI.Binding;
using Aci.Unity.Data;
using System;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.UserInterface.ValueConverters
{
    [CreateAssetMenu(menuName = "ACI/Value Converters/Badges To GradientConverter")]
    public class BadgesToGradientConverter : ScriptableObject, IValueConverter
    {
        [SerializeField]
        private Color[] m_Colors = new Color[4]; // TODO: get from style

        [SerializeField]
        private Gradient m_NoBadgesGradient;

        public object Convert(object value)
        {
            if (value == null)
                return m_NoBadgesGradient;

            if (!(value is BadgeData))
                throw new ArgumentException($"Expected {typeof(BadgeData)}.", nameof(value));

            Gradient gradient = new Gradient();
            BadgeData badges = (BadgeData) value;

            if (badges.Equals(default(BadgeData)))
                return m_NoBadgesGradient;

            // fast
            int fastCount = badges.TimeBadges.x + badges.TimeBadges.y + badges.TimeBadges.z;
            // streak
            int streakCount = badges.StreakBadges.x + badges.StreakBadges.y + badges.StreakBadges.z;
            // amount
            int amountCount = badges.AmountBadges.x + badges.AmountBadges.y + badges.AmountBadges.z;

            int total = fastCount + streakCount + amountCount;

            int tier1Count = badges.GetTotalCount(1);
            int tier2Count = badges.GetTotalCount(2);

            float tier1Normalized = (float)tier1Count / total;
            float tier2Normalized = (float)tier2Count / total;


            gradient.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(m_Colors[0], 0f),
                new GradientColorKey(m_Colors[1], tier1Normalized),
                new GradientColorKey(m_Colors[2], tier1Normalized + tier2Normalized),
                new GradientColorKey(m_Colors[3], 1f)
            };

            return gradient;
        }

        public object ConvertBack(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
