using System.Threading.Tasks;
using UnityEngine;
using UnityAsyncAwaitUtil;

namespace Aci.Unity.UserInterface.Animation
{
    public class QueueableParticleAnimation : MonoBehaviour, IQueueableAnimation
    {
        [SerializeField]
        private ParticleSystem m_ParticleSystem;

        private void Awake()
        {
            m_ParticleSystem.Stop();
        }

        public void OnEnable()
        {
            m_ParticleSystem.Stop();
        }

        public async Task Play()
        {
            m_ParticleSystem.time = 0;
            m_ParticleSystem.Play();
            await new WaitUntil(() => { return m_ParticleSystem.particleCount > 0; });
            await new WaitUntil(() => { return m_ParticleSystem.particleCount == 0; });
            m_ParticleSystem.Stop();
        }
    }
}