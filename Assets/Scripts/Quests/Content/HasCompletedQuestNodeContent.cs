using UnityEngine;

namespace Aci.Unity.Quests
{
    public class HasCompletedQuestNodeContent : QuestContent
    {
        [SerializeField]
        private string m_TitleKey;

        public bool isCompleted => quest.startNode.state == QuestNodeState.Succeeded;

        public string titleKey => m_TitleKey;
    }
}

