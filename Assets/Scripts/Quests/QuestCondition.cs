using UnityEngine;

namespace Aci.Unity.Quests
{
    public delegate void QuestConditionMetDelegate(QuestCondition questCondition);

    public abstract class QuestCondition : ScriptableObject
    {
        protected QuestNode questNode { get; private set; }

        protected Quest quest { get; private set; }

        private QuestConditionMetDelegate m_OnConditionMetCallback;

        /// <summary>
        /// Initializes the Quest Condition.
        /// </summary>
        /// <param name="quest">The quest this quest condition is apart of.</param>
        /// <param name="node">The quest node the quest condition is part of.</param>
        public virtual void Initialize(Quest quest, QuestNode node)
        {
            this.quest = quest;
            this.questNode = node;
        }

        public virtual void StartChecking(QuestConditionMetDelegate conditionMetCallback)
        {
            m_OnConditionMetCallback = conditionMetCallback;
        }

        public virtual void StopChecking() { }

        protected virtual void SetTrue()
        {
            m_OnConditionMetCallback?.Invoke(this);
        }
    }
}
