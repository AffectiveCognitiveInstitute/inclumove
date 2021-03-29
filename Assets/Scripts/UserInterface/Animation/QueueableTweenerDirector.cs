using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aci.Unity.UI.Tweening;
using System.Threading.Tasks;

namespace Aci.Unity.UserInterface.Animation
{
    public class QueueableTweenerDirector : MonoBehaviour, IQueueableAnimation
    {
        [SerializeField]
        private TweenerDirector m_TweenerDirector;

        public async Task Play()
        {
            m_TweenerDirector.Seek(0);
            await m_TweenerDirector.PlayForwardsAsync();
        }
    }
}
