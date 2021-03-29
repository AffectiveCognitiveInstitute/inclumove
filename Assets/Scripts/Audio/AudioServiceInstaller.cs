using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Audio
{
    public class AudioServiceInstaller : MonoInstaller<AudioServiceInstaller>
    {
        [SerializeField]
        private AudioServiceSettings m_AudioSettings;

        public override void InstallBindings()
        {
            Container.Bind<IAudioService>().ToSelf().FromInstance(new AudioService(m_AudioSettings)).AsSingle();
        }
    }
}
