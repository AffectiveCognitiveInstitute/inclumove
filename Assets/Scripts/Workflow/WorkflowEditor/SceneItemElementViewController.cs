using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Aci.Unity.Scene;
using Aci.Unity.Scene.SceneItems;
using Aci.Unity.UI;
using Unity.Transforms;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class SceneItemElementViewControllerFactory : IFactory<SceneItemTemplateData, string, UnityEngine.Object, SceneItemElementViewController>
    {
        readonly DiContainer _container;

        public SceneItemElementViewControllerFactory(DiContainer container)
        {
            _container = container;
        }

        public SceneItemElementViewController Create(SceneItemTemplateData data, string payload, UnityEngine.Object prefab)
        {
            SceneItemElementViewController controller =
                _container.InstantiatePrefab(prefab).GetComponent<SceneItemElementViewController>();
            controller.sceneItemTemplateData = data;
            controller.payload = payload;

            if (controller.sceneItemTemplateData.payloadType == PayloadType.Primitive)
            {
                controller.elementNameLabel.text = controller.sceneItemTemplateData.itemName;
            }
            else
            {
                int pathIndex = payload.LastIndexOf("/");
                controller.elementNameLabel.text = payload.Substring(pathIndex + 1, payload.Length - pathIndex - 5);
            }

            controller.UpdateSprite();

            return controller;
        }
    }

    public class SceneItemElementViewController : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<SceneItemTemplateData, string, UnityEngine.Object, SceneItemElementViewController>
        {
        }

        public TextMeshProUGUI elementNameLabel;
        public Image elementIcon;
        public Selectable selectable;
        public CachedImageComponent cachedUrlImage;

        [Tooltip("Template data used for SceneItem creation.")]
        public SceneItemTemplateData sceneItemTemplateData;
        [Tooltip("Payload used for SceneItem creation.")]
        public string payload;

        private IMemoryPool m_Pool;

        public void ToggleSelectable(bool enabled)
        {
            selectable.interactable = enabled;
        }

        public void UpdateSprite()
        {
            elementIcon.sprite = sceneItemTemplateData.placeholderIcon;
            if (sceneItemTemplateData.payloadType == PayloadType.Image && File.Exists(payload))
            {
                cachedUrlImage.url = "file://" + payload;
            }
        }
    }

}
