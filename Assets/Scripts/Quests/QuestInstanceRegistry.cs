using System;
using System.Collections.Generic;
using System.Linq;

namespace Aci.Unity.Quests
{
    public class QuestInstanceRegistry : IQuestInstanceRegistry
    {
        private Dictionary<string, List<Quest>> m_QuestInstances = new Dictionary<string, List<Quest>>();

        public Quest GetQuestById(string questId)
        {
            if (string.IsNullOrWhiteSpace(questId))
                throw new ArgumentNullException(nameof(questId));

            if (m_QuestInstances.TryGetValue(questId, out List<Quest> quests))
                return quests.FirstOrDefault();

            return null;
        }

        public void Register(Quest instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            string id = instance.id;

            if (m_QuestInstances.TryGetValue(id, out List<Quest> quests))
                quests.Add(instance);
            else
            {
                m_QuestInstances.Add(id, new List<Quest>() { instance });
            }
        }

        public void Unregister(Quest instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            string id = instance.id;

            if(m_QuestInstances.TryGetValue(id, out List<Quest> quests))
            {
                quests.Remove(instance);
                if (quests.Count == 0)
                    m_QuestInstances.Remove(id);
            }
        }
    }

}
