using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aci.Unity.Quests
{
    [System.Serializable]
    public class QuestContentCollection : IEnumerable<QuestContent>
    {
        [SerializeField]
        private List<QuestContent> m_Content;

        public void Initialize(Quest quest)
        {
            foreach (QuestContent c in m_Content)
                c.Initialize(quest);
        }

        public IEnumerator<QuestContent> GetEnumerator()
        {
            foreach (QuestContent c in m_Content)
                yield return c;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
