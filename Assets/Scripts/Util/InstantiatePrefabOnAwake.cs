using UnityEngine;
using Zenject;

namespace Aci.Unity.Util
{
    public class InstantiatePrefabOnAwake : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_Prefab;

        [SerializeField]
        private Transform m_Parent;
        private IInstantiator m_Instantiator;

        [Zenject.Inject]
        private void Construct(IInstantiator instantiator)
        {
            m_Instantiator = instantiator;
        }

        private void Awake()
        {
            if (m_Prefab == null)
                throw new MissingReferenceException("Unable to instantiate Prefab. A reference to the prefab is missing!");

            Execute();
        }

        private void Execute()
        {
            GameObject go = m_Instantiator.InstantiatePrefab(m_Prefab);
            if (m_Parent != null)
                go.transform.SetParent(m_Parent, false);
        }
    }
}