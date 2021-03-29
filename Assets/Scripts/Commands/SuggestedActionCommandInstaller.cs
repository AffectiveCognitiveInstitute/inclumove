using Zenject;

namespace Aci.Unity.Commands
{
    public class SuggestedActionCommandInstaller : MonoInstaller<SuggestedActionCommandInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<SuggestedActionCommand>().To<ReplyCommand>().AsCached();
            Container.Bind<SuggestedActionCommand>().To<IncreaseAdaptivityLevelCommand>().AsCached();
            Container.Bind<SuggestedActionCommand>().To<DecreaseAdaptivityLevelCommand>().AsCached();
            Container.Bind<SuggestedActionCommand>().To<IgnoreAdaptivityLevelCommand>().AsCached();

            Container.Bind<SuggestedActionCommandLibrary>().ToSelf().AsCached();
        }
    }
}