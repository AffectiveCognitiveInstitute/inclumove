using Zenject;
using Aci.Unity.Gamification;

namespace Aci.Unity.Workflow
{
    public class BadgeServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BadgeService>().FromNew().AsSingle();
        }
    }
}
