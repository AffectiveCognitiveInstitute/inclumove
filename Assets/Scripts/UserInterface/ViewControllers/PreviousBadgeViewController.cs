using Aci.Unity.Data;
using System;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class PreviousBadgeViewController : DayPerformanceViewController
    {
        public class Factory : PlaceholderFactory<DateTime, BadgeData, PreviousBadgeViewController> { }

        [Zenject.Inject]
        private void Construct(DateTime day,
                               BadgeData badges)
        {
            Initialize(day, badges);
        }
    }
}