using System;
using Zenject;

namespace Aci.Unity.Adaptivity
{
    public class AdaptivityServiceInstaller : MonoInstaller<AdaptivityServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind(typeof(IAdaptivityService), typeof(IDisposable), typeof(IInitializable)).To<AdaptivityService>().AsSingle();
        }
    }
}