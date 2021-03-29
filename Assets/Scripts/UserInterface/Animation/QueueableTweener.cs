using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aci.Unity.UI.Tweening;
using System.Threading.Tasks;

namespace Aci.Unity.UserInterface.Animation
{
    public class QueueableTweener : MonoBehaviour, IQueueableAnimation
    {
        [SerializeField]
        private Tweener m_Tweener;

        [SerializeField]
        private bool m_PlayForwards = true;

        public async Task Play()
        {
            if(m_PlayForwards)
                await m_Tweener.PlayForwardsAsync();
            else
                await m_Tweener.PlayReverseAsync();
            m_Tweener.Stop();
        }
    }  
}
