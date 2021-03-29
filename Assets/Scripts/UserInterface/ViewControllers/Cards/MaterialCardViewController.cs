using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class MaterialCardViewController : ImageCardViewController
    {
        private MonoMemoryPool<AssemblyComponentViewController> m_AssemblyPool;

        [SerializeField]
        private Transform m_MaterialContainer;

        [Zenject.Inject]
        private void Construct(MonoMemoryPool<AssemblyComponentViewController> assemblyPool)
        {
            m_AssemblyPool = assemblyPool;
        }

        public void Initialize(string message, string imageUrl, IEnumerable<AssemblyComponent> components)
        {
            this.message = message;
            this.imageUrl = imageUrl;

            foreach(var component in components)
            {
                var vc = m_AssemblyPool.Spawn();
                vc.Initialize(component);
                vc.transform.SetParent(m_MaterialContainer, false);
            }

            if (m_MaterialContainer.childCount == 0)
                m_MaterialContainer.gameObject.SetActive(false);
        }
    }
}

