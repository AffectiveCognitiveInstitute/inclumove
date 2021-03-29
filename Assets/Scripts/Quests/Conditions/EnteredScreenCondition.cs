using Aci.Unity.Events;
using Aci.Unity.UI.Navigation;
using UnityEngine;
using System;

namespace Aci.Unity.Quests
{
    public class EnteredScreenCondition : QuestCondition, IAciEventHandler<NavigationCompletedEvent>
    {
        [SerializeField]
        private string m_ScreenId;

        private IAciEventManager m_AciEventManager;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager)
        {
            m_AciEventManager = eventManager;
        }

        public override void StartChecking(QuestConditionMetDelegate conditionMetCallback)
        {
            base.StartChecking(conditionMetCallback);
            RegisterForEvents();
        }

        public override void StopChecking()
        {
            UnregisterFromEvents();
        }

        public void RegisterForEvents()
        {
            m_AciEventManager.AddHandler(this);
        }

        public void UnregisterFromEvents()
        {
            m_AciEventManager.RemoveHandler(this);
        }

        public void OnEvent(NavigationCompletedEvent arg)
        {
            if (arg.current.Equals(m_ScreenId, StringComparison.OrdinalIgnoreCase))
                SetTrue();
        }
    }
}

