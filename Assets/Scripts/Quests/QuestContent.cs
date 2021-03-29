using UnityEngine;

namespace Aci.Unity.Quests
{
    public abstract class QuestContent : ScriptableObject
    {
        public Quest quest { get; protected set; }

        public virtual void Initialize(Quest quest)
        {
            this.quest = quest;
        }
    }
}
