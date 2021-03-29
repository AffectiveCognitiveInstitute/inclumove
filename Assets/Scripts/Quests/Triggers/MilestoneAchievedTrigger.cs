using Aci.Unity.Data;
using Aci.Unity.Events;
using UnityEngine;

namespace Aci.Unity.Quests
{
    public class MilestoneAchievedTrigger : QuestTrigger
    {
        [SerializeField]
        private MilestoneData m_MilestoneData;

        private IAciEventManager m_AciEventManager;

        [Zenject.Inject]
        private void Construct(IAciEventManager aciEventManager)
        {
            m_AciEventManager = aciEventManager;
        }

        public override void Execute()
        {
            m_AciEventManager.Invoke(new MilestoneAchievedArgs() { milestone = m_MilestoneData });
        }
    }
}