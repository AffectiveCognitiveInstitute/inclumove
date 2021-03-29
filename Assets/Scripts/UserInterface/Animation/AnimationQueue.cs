using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Aci.Unity.UserInterface.Animation
{
    public class AnimationQueue : IAnimationQueue
    {
        private Queue<IQueueableAnimation> m_QueuedAnimations = new Queue<IQueueableAnimation>();
        
        public void Enqueue(IQueueableAnimation animation)
        {
            m_QueuedAnimations.Enqueue(animation);
        }

        public async Task Play()
        {
            while(m_QueuedAnimations.Count > 0)
            {
                IQueueableAnimation animation = m_QueuedAnimations.Dequeue();
                await animation.Play();
                (animation as QueueableAnimationSynchronizer)?.Dispose();
            }
        }

        public async Task PlaySafe()
        {
            IQueueableAnimation[] animations = m_QueuedAnimations.ToArray();
            foreach(IQueueableAnimation animation in animations)
            {
                await animation.Play();
            }
        }

        public void Clear()
        {
            while(m_QueuedAnimations.Count > 0)
            {
                IQueueableAnimation animation = m_QueuedAnimations.Dequeue();
                (animation as QueueableAnimationSynchronizer)?.Dispose();
            }
        }
    }
}