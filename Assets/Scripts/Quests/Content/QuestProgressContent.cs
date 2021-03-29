using UnityEngine;

namespace Aci.Unity.Quests
{
    public class QuestProgressContent : QuestContent
    {
        [SerializeField]
        private string m_TitleKey;

        [SerializeField]
        private string m_CurrentCounterId;

        [SerializeField]
        private string m_TargetCounterId;

        public string titleKey => m_TitleKey;

        public QuestCounter currentValue { get; private set; }

        public QuestCounter targetValue { get; private set; }

        public override void Initialize(Quest quest)
        {
            base.Initialize(quest);
            currentValue = quest.GetCounterById(m_CurrentCounterId);
            targetValue = quest.GetCounterById(m_TargetCounterId);
        }
    }
}