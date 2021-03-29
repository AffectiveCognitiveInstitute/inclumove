using Aci.Unity.Data;
using Aci.Unity.Events;
using UnityEngine;

namespace Aci.Unity.Gamification
{
    public class BadgeTracker : IUserMetricsTracker, IAciEventHandler<BadgeAmountCountChangedEvent>
    {
        private IBadgeService m_BadgeService;
        private IUserManager m_UserManager;
        private IAciEventManager m_EventManger;

        public BadgeTracker(IBadgeService badgeService, IUserManager userManager, IAciEventManager eventManager)
        {
            m_BadgeService = badgeService;
            m_UserManager = userManager;
            m_EventManger = eventManager;

            RegisterForEvents();
        }

        ~BadgeTracker()
        {
            UnregisterFromEvents();
        }

        /// <inheritdoc/>
        public void OnEvent(UserArgs arg)
        {
            if (arg.eventType != UserArgs.UserEventType.Save)
                return;
            UpdateUserData();

        }

        public void OnEvent(BadgeAmountCountChangedEvent arg)
        {
            UpdateUserData();
        }

        /// <inheritdoc/>
        public void RegisterForEvents()
        {
            m_EventManger.AddHandler<UserArgs>(this);
            m_EventManger.AddHandler<BadgeAmountCountChangedEvent>(this);
        }

        /// <inheritdoc/>
        public void UnregisterFromEvents()
        {
            m_EventManger.RemoveHandler<UserArgs>(this);
            m_EventManger.RemoveHandler<BadgeAmountCountChangedEvent>(this);
        }

        /// <inheritdoc/>
        public void UpdateUserData()
        {
            BadgeData badgeData = m_BadgeService.currentBadges;
            IUserProfile currentUser = m_UserManager.CurrentUser;
            currentUser.totalBadgeCount += badgeData.GetWeightedAmountTotalCount();
        }
    }
}

