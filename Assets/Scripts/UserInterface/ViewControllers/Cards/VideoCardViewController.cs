using System;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class VideoCardViewController : TextCardViewController, IPoolable<IMemoryPool>, IDisposable
    {
        public class Factory : PlaceholderFactory<VideoCardViewController> { }

        [SerializeField]
        private VideoPlayerViewController m_VideoPlayerViewController;
        private IMemoryPool m_Pool;

        public void Initialize(string message, string videoUrl)
        {
            this.message = message;
            m_VideoPlayerViewController.SetUrl(videoUrl);
        }

        public void Dispose()
        {
            m_Pool.Despawn(this);
        }

        public void OnDespawned()
        {
            m_Pool = null;
        }

        public void OnSpawned(IMemoryPool pool)
        {
            m_Pool = pool;
        }
    }
}
