using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace Aci.Unity.UserInterface.Animation
{
    public class QueueableAnimationSynchronizer : IQueueableAnimation, IDisposable
    {
        private List<IQueueableAnimation> m_Animations = new List<IQueueableAnimation>();

        public static readonly StaticMemoryPool<QueueableAnimationSynchronizer> Pool = new StaticMemoryPool<QueueableAnimationSynchronizer>(OnSpawned, OnDespawned);

        static void OnSpawned(QueueableAnimationSynchronizer that)
        {
        }

        static void OnDespawned(QueueableAnimationSynchronizer that)
        {
            that.m_Animations.Clear();
        }

        public void Dispose()
        {
            Pool.Despawn(this);
        }

        public void Append(IQueueableAnimation animation)
        {
            m_Animations.Add(animation);
        }

        public void Append(params IQueueableAnimation[] animations)
        {
            m_Animations.AddRange(animations);
        }

        public async Task Play()
        {
            await Task.WhenAll(m_Animations.Select(anim => anim.Play()));
        }
    }
}
