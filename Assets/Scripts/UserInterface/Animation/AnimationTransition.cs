using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Aci.Unity.UserInterface.Animation
{
    [RequireComponent(typeof(UnityEngine.Animation))]
    public class AnimationTransition : MonoBehaviour, IAnimatedTransition
    {
        [SerializeField]
        private AnimationClip m_EnterClip;
        [SerializeField]
        private AnimationClip m_ExitClip;
        private UnityEngine.Animation m_Animation;

        private void Awake()
        {
            m_Animation = GetComponent<UnityEngine.Animation>();
        }

        public Task PlayEnterAsync()
        {
            return PlayClip(m_EnterClip);
        }

        public Task PlayExitAsync()
        {
            return PlayClip(m_ExitClip);
        }

        private Task PlayClip(AnimationClip clip)
        {
            if (clip == null)
                return Task.CompletedTask;

            m_Animation.clip = clip;
            m_Animation.Play();
            return Task.Delay(TimeSpan.FromSeconds(clip.length));
        }
    }
}
