using System;
using Zenject;

namespace Aci.Unity.Data
{
    class IdProviderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IIdProviderService<int>>().To<IntegerIncrementService>().AsSingle();
            Container.Bind<IIdProviderService<Guid>>().To<GuidService>().AsSingle();
        }
    }
}
