using System.Collections;
using System.Collections.Generic;
using Aci.Unity.Scene;
using UnityEngine;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class DestroyHandle : MonoBehaviour, ISubHandle
    {
        private ISceneItemRegistry m_ItemRegistry;
        private SelectionHandle m_ParentHandle;

        void Awake()
        {
            m_ParentHandle = transform.parent.GetComponent<SelectionHandle>();
            handleActive.AddListener(m_ParentHandle.OnHandleActivated);
        }

        [Zenject.Inject]
        private void Construct(ISceneItemRegistry registry)
        {
            m_ItemRegistry = registry;
        }

        public SelectionHandle.HandleActiveEvent handleActive { get; } = new SelectionHandle.HandleActiveEvent();

        public void ToggleActive(bool enabled, bool silent)
        {
            // we won't need this since active means we destroy the object
        }

        public void ToggleHidden(bool hidden)
        {
            gameObject.SetActive(!hidden);
        }

        public void DestroyTarget()
        {
            m_ItemRegistry.RemoveItemById(m_ParentHandle.target.identifiable.identifier);
        }
    }
}
