using UnityEngine;

namespace Aci.Unity.Quests
{
    public abstract class QuestTrigger : ScriptableObject
    {
        /// <summary>
        /// The quest the trigger is part of. Set at runtime.
        /// </summary>
        protected Quest quest { get; private set; }

        /// <summary>
        /// The quest node this trigger is part of. Set at runtime.
        /// </summary>
        protected QuestNode questNode { get; private set; }

        public virtual void Initialize(Quest quest, QuestNode questNode)
        {
            this.quest = quest;
            this.questNode = questNode;
        }

        public abstract void Execute();
    }
}