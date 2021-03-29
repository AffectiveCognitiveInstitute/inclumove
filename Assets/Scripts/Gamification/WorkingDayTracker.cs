
using Aci.Unity.Events;
using Aci.Unity.Util;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Gamification
{
    public class WorkingDayTracker : IUserMetricsTracker
    {
        private ITimeProvider m_TimeProvider;
        private IAciEventManager m_EventManger;
        private IUserManager m_UserManager;

        public WorkingDayTracker(ITimeProvider timeProvider,
                              IAciEventManager eventManager,
                              IUserManager userManager)
        {
            m_TimeProvider = timeProvider;
            m_EventManger = eventManager;
            m_UserManager = userManager;

            RegisterForEvents();
        }

        ~WorkingDayTracker()
        {
            UnregisterFromEvents();
        }

        /// <inheritdoc/>
        public void OnEvent(UserArgs arg)
        {
            if (arg.eventType != UserArgs.UserEventType.Login)
                return;
            UpdateUserData();
        }

        /// <inheritdoc/>
        public void RegisterForEvents()
        {
            m_EventManger.AddHandler(this);
        }

        /// <inheritdoc/>
        public void UnregisterFromEvents()
        {
            m_EventManger.RemoveHandler(this);
        }

        /// <inheritdoc/>
        public void UpdateUserData()
        {
            // get user data from user manager
            IUserProfile currentUser = m_UserManager.CurrentUser;
            // get the last login date of user
            string lastLoginDate = currentUser.lastLoginDate;
            
            string date = System.DateTime.Now.ToShortDateString();
            // compare last login date to current login date
            if (lastLoginDate?.Equals(date) ?? false) 
                return;

            currentUser.lastLoginDate = date;
            int prevDayCount = currentUser.numberOfDaysWorked;
            currentUser.numberOfDaysWorked += 1;
            m_EventManger.Invoke(new WorkingDayCountChangedEvent
            {
                PrevCount = prevDayCount,
                NewCount = currentUser.numberOfDaysWorked
            });
        }        
    }
}

