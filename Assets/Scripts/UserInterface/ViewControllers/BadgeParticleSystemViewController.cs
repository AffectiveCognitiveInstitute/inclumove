using UnityEngine;
using Zenject;

namespace Aci.Unity.UI.ViewControllers
{
    [RequireComponent(typeof(ParticleSystem))]
    public class BadgeParticleSystemViewController : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<int, BadgeParticleSystemViewController> { }

        private ParticleSystem m_ParticleSystem;
        public new ParticleSystem particleSystem
        {
            get
            {
                if (m_ParticleSystem == null)
                    m_ParticleSystem = GetComponent<ParticleSystem>();

                return m_ParticleSystem;
            }
        }

        [Zenject.Inject]
        private void Construct(int badgesCount)
        {
            SetParticleCount(badgesCount);
        }

        private void SetParticleCount(int badgesCount)
        {
            ParticleSystem.MainModule mainModule = particleSystem.main;
            mainModule.maxParticles = badgesCount;
        }
    }
}