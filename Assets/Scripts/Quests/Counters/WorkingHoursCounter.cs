using Aci.Unity.Events;
using Aci.Unity.Gamification;
using UnityEngine;

namespace Aci.Unity.Quests
{
    public class WorkingHoursCounter : QuestCounter, IAciEventHandler<WorkingHourCountChangedEvent>
    {
        private IAciEventManager m_EventManager;
        private IUserManager m_UserManager;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager, IUserManager userManager)
        {
            m_EventManager = eventManager;
            m_UserManager = userManager;
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler(this);
        }

        public override void StartListening()
        {
            value = m_UserManager.CurrentUser.accumulatedTime.Hours;
            RegisterForEvents();
        }

        public override void StopListening()
        {
            UnregisterFromEvents();
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler(this);
        }

        public void OnEvent(WorkingHourCountChangedEvent arg)
        {
            value = Mathf.FloorToInt(arg.SpentTime);
        }
    }
}

