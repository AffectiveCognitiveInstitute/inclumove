using Zenject;

namespace Aci.Unity.UserInterface
{
    public class ChatWindowInstaller : MonoInstaller<ChatWindowInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IChatWindowFacade>().To<ChatWindowFacade>().AsSingle();
        }
    }
}