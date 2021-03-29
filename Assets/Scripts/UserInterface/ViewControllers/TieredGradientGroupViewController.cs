using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class TieredGradientGroupViewController : MonoBehaviour
    {
        [SerializeField]
        private Gradient2[] m_Gradients;

        [SerializeField]
        private UnityEngine.Gradient[] m_GradientTiers;

        public void SetTier(uint tier)
        {
            if (tier >= m_GradientTiers.Length)
                return;
            
            foreach(Gradient2 targetGradient in m_Gradients)
            {
                targetGradient.EffectGradient = m_GradientTiers[tier];
            }
        }
    }
}
