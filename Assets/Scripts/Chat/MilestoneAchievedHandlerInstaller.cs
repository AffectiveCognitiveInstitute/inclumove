using Aci.Unity.Data;
using Microsoft.Bot.Connector.DirectLine;
using Zenject;

namespace Aci.Unity.Chat
{
    public class MilestoneAchievedHandlerInstaller : MonoInstaller<MilestoneAchievedHandlerInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindFactory<MilestoneData, Activity, MilestoneAchievedActivityFactory>();
        }
    }
}