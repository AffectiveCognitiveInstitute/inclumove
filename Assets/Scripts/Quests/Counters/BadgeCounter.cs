using Aci.Unity.Events;
using Aci.Unity.Gamification;

namespace Aci.Unity.Quests
{
    public class BadgeCounter : QuestCounter, IAciEventHandler<BadgeAmountCountChangedEvent>
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
            this.value = m_UserManager.CurrentUser.totalBadgeCount;
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

        public void OnEvent(BadgeAmountCountChangedEvent arg)
        {
            this.value = arg.TotalCount;
        }

    }
}

