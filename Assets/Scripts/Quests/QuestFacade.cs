using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Quests
{
    public class QuestFacade : IQuestFacade
    {
        private DiContainer m_DiContainer;
        private IQuestInstanceRegistry m_QuestInstanceRegistry;

        public QuestFacade(DiContainer diContainer, IQuestInstanceRegistry questInstanceRegistry)
        {
            m_DiContainer = diContainer;
            m_QuestInstanceRegistry = questInstanceRegistry;
        }

        /// <inheritdoc/>
        public Quest GetInstance(string questId)
        {
            return m_QuestInstanceRegistry.GetQuestById(questId);
        }

        /// <inheritdoc/>
        public QuestState GetQuestState(string questId)
        {
            Quest questInstance = m_QuestInstanceRegistry.GetQuestById(questId);
            
            if(questInstance != null)
                return questInstance.state;

            return QuestState.Inactive;
        }

        /// <inheritdoc/>
        public Quest StartQuest(Quest quest)
        {
            Quest questInstance = quest.Instantiate();

            m_QuestInstanceRegistry.Register(questInstance);

            QuestNode startNode = questInstance.startNode;

            IReadOnlyList<QuestCondition> conditions = startNode.conditions;
            for (int i = 0; i < conditions.Count; i++)
                m_DiContainer.Inject(conditions[i]);

            IReadOnlyList<QuestTrigger> successTriggers = startNode.successTriggers;
            for (int i = 0; i < successTriggers.Count; i++)
                m_DiContainer.Inject(successTriggers[i]);

            IReadOnlyList<QuestTrigger> failedTriggers = startNode.failedTriggers;
            for (int i = 0; i < failedTriggers.Count; i++)
                m_DiContainer.Inject(failedTriggers[i]);

            foreach (QuestCounter counter in questInstance.counters)
                m_DiContainer.Inject(counter);

            questInstance.SetState(QuestState.Active);

            return questInstance;
        }

        /// <inheritdoc/>
        public void StopQuest(string questId)
        {
            if (string.IsNullOrWhiteSpace(questId))
                throw new System.ArgumentNullException(nameof(questId));

            Quest questInstance = m_QuestInstanceRegistry.GetQuestById(questId);

            if(questInstance != null)
            {
                questInstance.SetState(QuestState.Inactive);
                m_QuestInstanceRegistry.Unregister(questInstance);
                Object.Destroy(questInstance);
            }
        }
    }
}