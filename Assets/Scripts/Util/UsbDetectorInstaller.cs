using Zenject;

namespace Aci.Unity.Util
{
    public class UsbDetectorInstaller : MonoInstaller<UsbDetectorInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UsbDetectorService>().FromNew().AsSingle();
        }
    }
}