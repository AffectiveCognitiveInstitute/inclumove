using Aci.Unity.Events;
using Aci.Unity.Gamification;
using UnityEngine;

namespace Aci.Unity.Quests
{
    public class WorkingDaysCounter : QuestCounter, IAciEventHandler<WorkingDayCountChangedEvent>
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
            this.value = m_UserManager.CurrentUser.numberOfDaysWorked;
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

        public void OnEvent(WorkingDayCountChangedEvent arg)
        {
            this.value = arg.NewCount;
        }
    }
}
