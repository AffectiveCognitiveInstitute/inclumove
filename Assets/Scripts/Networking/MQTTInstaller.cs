using Zenject;

namespace Aci.Unity.Networking
{
    class MQTTInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MQTTConnector>().FromComponentInHierarchy().AsSingle();
        }
    }
}
