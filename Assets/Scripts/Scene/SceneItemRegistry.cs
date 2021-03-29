using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aci.Unity.Scene.SceneItems;
using UnityEngine;
using UnityEngine.Events;

namespace Aci.Unity.Scene
{
    public class SceneItemRegistry : ISceneItemRegistry
    {
        private class SceneItemEvent : UnityEvent<ISceneItem> { }

        private Dictionary<int, ISceneItem> m_SceneItems = new Dictionary<int, ISceneItem>();

        private Transform m_SceneItemTransform;

        public UnityEvent<ISceneItem> itemRemoved { get; } = new SceneItemEvent();
        public UnityEvent<ISceneItem> itemAdded { get; } = new SceneItemEvent();

        [Zenject.Inject]
        private void Construct([Zenject.Inject(Id = "SceneItemRoot")] Transform sceneItemRoot)
        {
            m_SceneItemTransform = sceneItemRoot;
        }


        /// <inheritdoc />
        public void RegisterItem(ISceneItem item)
        {
            if (m_SceneItems.ContainsKey(item.identifiable.identifier))
                return;

            item.itemTransform.SetParent(m_SceneItemTransform, false);
            m_SceneItems.Add(item.identifiable.identifier, item);
            itemAdded.Invoke(item);
        }

        /// <inheritdoc />
        public ISceneItem GetItemById(int id)
        {
            if (!m_SceneItems.ContainsKey(id))
                return null;
            return m_SceneItems[id];
        }

        /// <inheritdoc />
        public void RemoveItemById(int id)
        {
            if (!m_SceneItems.ContainsKey(id))
                return;
            ISceneItem sceneItem = m_SceneItems[id];
            sceneItem.itemTransform.SetParent(null);
            m_SceneItems.Remove(id);
            itemRemoved.Invoke(sceneItem);
            GameObject.Destroy(sceneItem.itemTransform.gameObject);
        }

        /// <inheritdoc />
        public void ClearRegistry()
        {
            foreach (KeyValuePair<int, ISceneItem> kvp in m_SceneItems)
            {
                ISceneItem sceneItem = kvp.Value;
                sceneItem.itemTransform.SetParent(null);
                itemRemoved.Invoke(sceneItem);
                GameObject.Destroy(sceneItem.itemTransform.gameObject);
            }
            m_SceneItems.Clear();
        }
    }
}
