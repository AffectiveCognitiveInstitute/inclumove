using Zenject;

namespace Aci.Unity.Chat
{
    public class AdaptivityActivityHandlerInstaller : MonoInstaller<AdaptivityActivityHandlerInstaller>
    {
        [Zenject.Inject]
        private AdaptivityActivityLibrary m_Libary;

        public override void InstallBindings()
        {
            Container.Bind<AdaptivityActivityLibrary>().FromInstance(m_Libary).AsCached();
        }
    }
}