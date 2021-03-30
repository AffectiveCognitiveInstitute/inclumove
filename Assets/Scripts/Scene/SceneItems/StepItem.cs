using System;
using System.Collections;
using System.Collections.Generic;
using Aci.Unity.Data;
using Aci.Unity.Data.JsonModel;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Scene.SceneItems
{
    public class StepItem : MonoBehaviour, IStepItem
    {
        public class Factory : PlaceholderFactory<WorkflowStepData, UnityEngine.Object, IStepItem> { }

        private ISceneItemRegistry m_ItemRegistry;

        private List<ISceneItem> m_SceneItems = new List<ISceneItem>();
        public IReadOnlyList<ISceneItem> sceneItems => m_SceneItems;

        private IIdentifiable<uint> m_Identifiable;
        public IIdentifiable<uint> identifiable => m_Identifiable;
        public float3 duration { get; set; }
        public int repetitions { get; set; }
        public bool automatic { get; set; }
        public bool mount { get; set; }
        public bool unmount { get; set; }
        public bool control { get; set; }
        public string name { get; set; }
        public int partId { get; set; }
        public int gripperId { get; set; }

        [Zenject.Inject]
        private void Construct(IIdentifiable<uint> identifiable, ISceneItemRegistry itemRegistry)
        {
            m_Identifiable = identifiable;
            m_ItemRegistry = itemRegistry;
            m_ItemRegistry.itemRemoved.AddListener(OnItemRemoved);
        }

        public bool Add(ISceneItem item)
        {
            if (m_SceneItems.Contains(item))
                return false;
            m_SceneItems.Add(item);
            return true;
        }

        public bool Remove(ISceneItem item)
        {
            if (!m_SceneItems.Contains(item))
                return true;
            m_ItemRegistry.RemoveItemById(item.identifiable.identifier);
            return true;
        }

        public void Dispose()
        {
            while(m_SceneItems.Count > 0)
                m_ItemRegistry.RemoveItemById(m_SceneItems[0].identifiable.identifier);
            GameObject.Destroy(gameObject);
            m_ItemRegistry.itemRemoved.RemoveListener(OnItemRemoved);
        }

        private void OnItemRemoved(ISceneItem arg0)
        {
            if (!m_SceneItems.Contains(arg0))
                return;
            m_SceneItems.Remove(arg0);
        }
    }
}
