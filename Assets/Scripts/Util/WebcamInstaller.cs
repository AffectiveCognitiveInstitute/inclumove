using Aci.Unity.Sensor;
using UnityEngine;
using Zenject;

public class WebcamInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<WebcamProvider>().FromComponentInHierarchy().AsSingle();
    }
}