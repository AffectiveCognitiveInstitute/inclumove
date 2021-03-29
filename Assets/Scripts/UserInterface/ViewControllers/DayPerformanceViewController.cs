using Aci.UI.Binding;
using Aci.Unity.Data;
using System;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class DayPerformanceViewController : MonoBindable
    {

        private DateTime m_Day;
        private BadgeData m_Badges;
        private int m_TotalBadgesCount;

        /// <summary>
        ///     The actual day the work day took place.
        /// </summary>
        public DateTime day
        {
            get => m_Day;
            set => SetProperty(ref m_Day, value);
        }

        /// <summary>
        ///     The badges accumulated during that day.
        /// </summary>
        public BadgeData badges
        {
            get => m_Badges;
            set => SetProperty(ref m_Badges, value);
        }

        /// <summary>
        ///     The total number of badges accumulated.
        /// </summary>
        public int totalBadgesCount
        {
            get => m_TotalBadgesCount;
            set => SetProperty(ref m_TotalBadgesCount, value);
        }

        /// <summary>
        ///     Initializes the ViewController.
        /// </summary>
        /// <param name="day">The day the workday took place.</param>
        /// <param name="badges">The <see cref="BadgeData"/> associated with the workday.</param>
        public void Initialize(DateTime day, BadgeData badges)
        {
            this.day = day;
            this.badges = badges;
            this.totalBadgesCount = badges.GetWeightedTotalCount();
        }
    }
}