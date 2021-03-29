using Aci.Unity.Services;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Util
{
    public class CachedImageInstaller : MonoInstaller<CachedImageInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<ICachedResourceProvider<Sprite, string>>().To<InMemoryCachedImageProvider>().AsSingle();
        }
    }
}
