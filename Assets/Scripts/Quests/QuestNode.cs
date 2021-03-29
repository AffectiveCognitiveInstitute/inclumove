using System.Collections.Generic;
using UnityEngine;

namespace Aci.Unity.Quests
{
    [System.Serializable]
    public class QuestNode
    {
        [SerializeField]
        private QuestConditionMode m_ConditionMode;

        [SerializeField]
        private List<QuestCondition> m_Conditions;

        [SerializeField]
        private List<QuestTrigger> m_SuccessTriggers;

        [SerializeField]
        private List<QuestTrigger> m_FailedTriggers;

        [SerializeField]
        private int m_MinimumNumberOfConditions;

        private QuestNodeState m_State = QuestNodeState.Inactive;
        private int m_SucceededConditionsCount = 0;

        /// <summary>
        ///     The quest node's state.
        /// </summary>
        public QuestNodeState state => m_State;

        /// <summary>
        ///     Have the required number of conditions been met?
        /// </summary>
        public bool areConditionsMet
        {
            get
            {
                if (m_Conditions == null || m_Conditions.Count == 0)
                    return true;

                if (m_SucceededConditionsCount == 0)
                    return false;

                switch(m_ConditionMode)
                {
                    case QuestConditionMode.All:
                        return m_SucceededConditionsCount >= m_Conditions.Count;
                    case QuestConditionMode.Any:
                        return m_SucceededConditionsCount > 0;
                    case QuestConditionMode.AtLeast:
                        return m_SucceededConditionsCount >= m_MinimumNumberOfConditions;
                }

                return false;
            }
        }

        /// <summary>
        ///     The quest node's conditions.
        /// </summary>
        public IReadOnlyList<QuestCondition> conditions => m_Conditions.AsReadOnly();

        /// <summary>
        ///     The quest node's success triggers.
        /// </summary>
        public IReadOnlyList<QuestTrigger> successTriggers => m_SuccessTriggers.AsReadOnly();

        /// <summary>
        ///     The quest node's failed triggers.
        /// </summary>
        public IReadOnlyList<QuestTrigger> failedTriggers => m_FailedTriggers.AsReadOnly();

        /// <summary>
        /// The quest this QuestNode belongs to. Set only at runtime.
        /// </summary>
        public Quest quest { get; private set; }

        /// <summary>
        /// Initializes the QuestNode.
        /// </summary>
        /// <param name="quest">The quest this quest node belongs to.</param>
        public void Initialize(Quest quest)
        {
            this.quest = quest;

            for(int i = 0; i < conditions.Count; i++)
                conditions[i].Initialize(quest, this);

            for (int i = 0; i < successTriggers.Count; i++)
                successTriggers[i].Initialize(quest, this);

            for (int i = 0; i < failedTriggers.Count; i++)
                failedTriggers[i].Initialize(quest, this);
        }

        public void SetState(QuestNodeState state)
        {
            if (m_State == state)
                return;

            m_State = state;
            SetConditionsActive(state == QuestNodeState.Active);

            switch (m_State)
            {
                case QuestNodeState.Succeeded:
                    ExecuteTriggerList(m_SuccessTriggers);
                    break;
                case QuestNodeState.Failed:
                    ExecuteTriggerList(m_FailedTriggers);
                    break;
                default:
                    break;
            }
        }

        private void ExecuteTriggerList(List<QuestTrigger> triggers)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                QuestTrigger t = triggers[i];
                if (t != null)
                    t.Execute();
            }
        }

        private void SetConditionsActive(bool active)
        {
            if (active)
            {
                for (int i = 0; i < m_Conditions.Count; i++)
                    m_Conditions[i].StartChecking(OnQuestConditionMet);
            }
            else
            {
                for (int i = 0; i < m_Conditions.Count; i++)
                    m_Conditions[i].StopChecking();
            }
        }

        private void OnQuestConditionMet(QuestCondition questCondition)
        {
            m_SucceededConditionsCount++;
            if (areConditionsMet)
                SetState(QuestNodeState.Succeeded);
        }
    }

    public enum QuestNodeState
    {
        Inactive,
        Active,
        Succeeded,
        Failed
    }

    public enum QuestConditionMode
    {
        /// <summary>
        ///     If any condition becomes true, the QuestNode becomes true.
        /// </summary>
        Any,

        /// <summary>
        ///     All conditions must become true for the QuestNode to become true.
        /// </summary>
        All,

        /// <summary>
        ///     At least a given of number of conditions must become true for the QuestNode to become true./>
        /// </summary>
        AtLeast
    }
}
