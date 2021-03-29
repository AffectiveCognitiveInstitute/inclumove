using UnityEngine;

namespace Aci.Unity.Quests
{
    public class CounterQuestCondition : QuestCondition
    {
        [SerializeField, Tooltip("The Id of the counter in the quest")]
        private string m_CounterId;

        [SerializeField, Tooltip("The Id of the required count.")]
        private string m_RequiredCountId;

        [SerializeField]
        private CounterConditionType m_Type;

        private QuestCounter m_Counter;
        private QuestCounter m_RequiredCount;

        public override void Initialize(Quest quest, QuestNode node)
        {
            base.Initialize(quest, node);
            m_Counter = quest.GetCounterById(m_CounterId);
            m_RequiredCount = quest.GetCounterById(m_RequiredCountId);
        }

        public override void StartChecking(QuestConditionMetDelegate conditionMetCallback)
        {
            base.StartChecking(conditionMetCallback);
            m_Counter.valueChanged += OnCounterValueChanged;
        }

        public override void StopChecking()
        {
            base.StopChecking();
            m_Counter.valueChanged -= OnCounterValueChanged;
        }

        private void OnCounterValueChanged(QuestCounter counter, int previousValue)
        {
            switch (m_Type)
            {
                case CounterConditionType.AtLeast:
                    if (m_Counter.value >= m_RequiredCount.value)
                        SetTrue();
                    break;
                case CounterConditionType.AtMost:
                    if (m_Counter.value <= m_RequiredCount.value)
                        SetTrue();
                    break;
            }
        }
    }

    public enum CounterConditionType
    {
        /// <summary>
        /// The count must reach at least this amount for the condition to become true.
        /// </summary>
        AtLeast,

        /// <summary>
        /// The count must not go higher
        /// </summary>
        AtMost
    }
}
