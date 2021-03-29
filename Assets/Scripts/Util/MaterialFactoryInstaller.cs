using Aci.Unity.UserInterface.ViewControllers;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Util
{
    public class MaterialFactoryInstaller : MonoInstaller<MaterialFactoryInstaller>
    {
        [SerializeField]
        private GameObject m_Prefab;
        
        public override void InstallBindings()
        {
            Container.BindMemoryPool<AssemblyComponentViewController, MonoMemoryPool<AssemblyComponentViewController>>().
                FromComponentInNewPrefab(m_Prefab);
        }
    }
}