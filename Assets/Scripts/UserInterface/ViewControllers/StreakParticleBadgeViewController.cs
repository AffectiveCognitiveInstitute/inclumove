using UnityEngine;
using Aci.Unity.UserInterface.Animation;
using Aci.Unity.UI.Tweening;
using UnityEngine.UI.Extensions;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class StreakParticleBadgeViewController : MonoBehaviour
    {
        [SerializeField]
        private AlphaTweener m_BlendInTweener;

        [SerializeField]
        private AlphaTweener m_BlendOutTweener;

        [SerializeField]
        private QueueableParticleAnimation m_QueueableParticleAnimation;

        [SerializeField]
        private UIParticleSystem m_ParticleSystem;

        [SerializeField]
        private Material[] m_ParticleSprayMaterials;

        public AlphaTweener blendInTweener => m_BlendInTweener;

        public AlphaTweener blendOutTweener => m_BlendOutTweener;

        public QueueableParticleAnimation queueableParticleAnimation => m_QueueableParticleAnimation;

        public void SetBadgeTier(int tier)
        {
            tier = Mathf.Clamp(tier, 0, 2);
            m_ParticleSystem.material = m_ParticleSprayMaterials[tier];
        }
    }
}