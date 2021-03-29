using Aci.Unity.Data;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Scene.SceneItems
{
    class SceneItemInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISceneItem>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IIdentifiable<int>>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IPayloadViewController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IColorable>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IScalable>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ILevelable>().To<Levelable>().AsSingle();
        }
    }
}
