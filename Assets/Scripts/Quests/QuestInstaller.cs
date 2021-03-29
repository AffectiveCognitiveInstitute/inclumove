using Zenject;

namespace Aci.Unity.Quests
{
    public class QuestInstaller : MonoInstaller<QuestInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IQuestInstanceRegistry>().To<QuestInstanceRegistry>().AsCached();
            Container.Bind<IQuestFacade>().To<QuestFacade>().AsCached();
        }
    }
}