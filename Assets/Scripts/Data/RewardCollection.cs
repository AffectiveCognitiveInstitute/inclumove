using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.Data
{
    [CreateAssetMenu(menuName = "Inclumove/Reward Collection")]
    public class RewardCollection : ScriptableObject, IEnumerable<RewardData>
    {
        [SerializeField]
        private List<RewardData> m_Rewards;

        public RewardData GetRewardById(string id)
        {
            return m_Rewards.FirstOrDefault(x => x.id == id);
        }

        public IEnumerator<RewardData> GetEnumerator()
        {
            foreach (RewardData d in m_Rewards)
                yield return d;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public RewardCollection CreateCopy()
        {
            RewardCollection collection = ScriptableObject.CreateInstance<RewardCollection>();
            collection.m_Rewards = new List<RewardData>();
            foreach (RewardData data in m_Rewards)
                collection.m_Rewards.Add(Instantiate(data));
            return collection;
        }
    }
}

