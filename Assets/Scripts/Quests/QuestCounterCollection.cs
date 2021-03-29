using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.Quests
{
    [System.Serializable]
    public class QuestCounterCollection : IEnumerable<QuestCounter>
    {
        [SerializeField]
        private List<QuestCounter> m_QuestCounters;

        public QuestCounter this[string id]
        {
            get => m_QuestCounters.FirstOrDefault(x => x.id == id);
        }

        public IEnumerator<QuestCounter> GetEnumerator()
        {
            foreach (QuestCounter c in m_QuestCounters)
                yield return c;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Initialize(Quest quest)
        {
            for (int i = 0; i < m_QuestCounters.Count; i++)
                m_QuestCounters[i].Initialize(quest);
        }

        public void StartListening()
        {
            for (int i = 0; i < m_QuestCounters.Count; i++)
                m_QuestCounters[i].StartListening();
        }

        public void StopListening()
        {
            for (int i = 0; i < m_QuestCounters.Count; i++)
                m_QuestCounters[i].StopListening();
        }
    }
}