using System;
using Aci.Unity.Data;
using Zenject;

namespace Aci.Unity.Scene.SceneItems
{
    class StepItemInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IStepItem>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IIdentifiable<uint>>().FromComponentInHierarchy().AsSingle();
        }
    }
}
