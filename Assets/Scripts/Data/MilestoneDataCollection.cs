using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.Data
{
    [CreateAssetMenu(menuName = "Inclumove/Milestone Collection")]
    public class MilestoneDataCollection : ScriptableObject, IEnumerable<MilestoneData>
    {
        [SerializeField]
        private List<MilestoneData> m_Milestones;

        public MilestoneData GetDataById(string id) => m_Milestones.FirstOrDefault(x => x.id == id);

        public IEnumerator<MilestoneData> GetEnumerator()
        {
            foreach (MilestoneData d in m_Milestones)
                yield return d;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}