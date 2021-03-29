using Aci.Unity.Data.JsonModel;
using Aci.Unity.Scene;
using Microsoft.Bot.Connector.DirectLine;
using Zenject;

namespace Aci.Unity.Util
{
    public class ActivityInstaller : MonoInstaller<ActivityInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindFactory<string, ActivityData, ActivityData.Factory>().FromFactory<ActivityDataFactory>();
            Container.BindFactory<ActivityData, Activity, ActivityFactory>().FromFactory<ConcreteActivityFactory>();
        }
    }
}

