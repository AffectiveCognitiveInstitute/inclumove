using Aci.Unity.Workflow;
using Aci.Unity.Events;
using Aci.Unity.Util;
using Zenject;
using UnityEngine;

namespace Aci.Unity.Gamification
{
    public class WorkingHourTracker : IUserMetricsTracker, ITickable
    {
        private TickableManager m_TickableManager;
        private ITimeProvider m_TimeProvider;
        private IAciEventManager m_EventManger;
        private IUserManager m_UserManager;
        private float m_LastTime = 0;
        private float m_UpdateRate = (float)System.TimeSpan.FromMinutes(5).TotalSeconds;
        private System.TimeSpan m_PrevElapsed = new System.TimeSpan();

        /// <summary>	
        /// Creates a <see cref="WorkingHourTracker"/> instance.	
        /// </summary>	
        /// <param name="tickableManager"><see cref="TickableManager"/> instance.</param> 
        /// <param name="timeProvider"><see cref="ITimeProvider"/> instance.</param>	
        /// <param name="eventManager"><see cref="IAciEventManager"/> instance.</param>	
        /// <param name="userManager"><see cref="IUserManager"/> instance.</param>

        [Inject]
        public WorkingHourTracker(TickableManager tickableManager, 
                              ITimeProvider timeProvider,
                              IAciEventManager eventManager,
                              IUserManager userManager)
        {
            m_TickableManager = tickableManager;
            m_EventManger = eventManager;
            m_TimeProvider = timeProvider;
            m_UserManager = userManager;
            m_TickableManager.Add(this);

            RegisterForEvents();
            
        }

        ~WorkingHourTracker()
        {
            m_TickableManager.Remove(this);
            UnregisterFromEvents();
        }

        /// <inheritdoc/>
        public void OnEvent(UserArgs arg)
        {
            if (arg.eventType != UserArgs.UserEventType.Save) 
            return;
            UpdateUserData();
        }

        /// <inheritdoc/>
        public void RegisterForEvents()
        {
            m_EventManger.AddHandler(this);
        }

        public void Tick()
        {
            if (Time.time - m_LastTime < m_UpdateRate)
                return;
            m_LastTime = Time.time;
            UpdateUserData();
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
            // get time data from time provider
            System.TimeSpan elapsedTotal = m_TimeProvider.elapsedTotal;
            // Get delta and make sure it's not negative (eg. after a workflow restart)
            System.TimeSpan delta = elapsedTotal - m_PrevElapsed;
            if (delta.TotalSeconds < 0)
                delta = System.TimeSpan.Zero;
            // save the time spent in userprofile
            currentUser.accumulatedTime += delta;
            // set the last set time
            m_PrevElapsed = elapsedTotal;

            m_EventManger.Invoke(new WorkingHourCountChangedEvent
            {
                SpentTime = (float)currentUser.accumulatedTime.TotalHours
            });
        }
    }
}
