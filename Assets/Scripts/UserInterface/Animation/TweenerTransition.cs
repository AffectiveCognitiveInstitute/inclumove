using Aci.Unity.UI.Tweening;
using System.Threading.Tasks;
using UnityEngine;

namespace Aci.Unity.UserInterface.Animation
{
    public class TweenerTransition : MonoBehaviour, IAnimatedTransition
    {
        [SerializeField]
        private TweenerDirector m_EnterDirector;

        [SerializeField]
        private TweenerDirector m_ExitDirector;

        public Task PlayEnterAsync()
        {
            if (m_EnterDirector == null)
                return Task.CompletedTask;

            return m_EnterDirector.PlayForwardsAsync(false);
        }

        public Task PlayExitAsync()
        {
            if (m_ExitDirector == null)
                return Task.CompletedTask;

            return m_ExitDirector.PlayForwardsAsync(false);
        }
    }
}
