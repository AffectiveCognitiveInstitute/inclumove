using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aci.Unity.Gamification.Rewards
{
    [CreateAssetMenu(menuName = "Inclumove/Reward Particle Systems")]
    public class RewardParticleSystems : ScriptableObject, IEnumerable<ParticleSystemData>
    {
        [SerializeField]
        private ParticleSystemData[] m_ParticleSystems;

        public IEnumerator<ParticleSystemData> GetEnumerator()
        {
            foreach (ParticleSystemData psd in m_ParticleSystems)
                yield return psd;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
