using Aci.Unity.Network;
using Zenject;

public class PingInstaller : MonoInstaller<PingInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<PingHandler>().FromComponentInHierarchy().AsSingle();
    }
}
