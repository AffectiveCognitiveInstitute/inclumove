// <copyright file=DragEventHandler.cs/>
// <copyright>
//   Copyright (c) 2019, Affective & Cognitive Institute
// 
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software andassociated documentation files
//   (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
//   merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
// 
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//   LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
//   IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <license>MIT License</license>
// <main contributors>
//   Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>10/24/2019 09:20</date>

using Aci.Unity.Data.JsonModel;
using Aci.Unity.Scene.SceneItems;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class DragEventHandler : MonoBehaviour
    {
        private SceneItem.Factory m_SceneItemFactory;

        [Inject]
        private void Construct(SceneItem.Factory factory)
        {
            m_SceneItemFactory = factory;
        }

        public void OnObjectGrabbed(ReorderableList.ReorderableListEventStruct e)
        {
            e.DroppedObject.transform.localScale = Vector3.one;
            SceneItemElementViewController source = e.SourceObject.GetComponent<SceneItemElementViewController>();
            SceneItemElementViewController controller = e.DroppedObject.GetComponent<SceneItemElementViewController>();
            ReorderableListElement listElement = e.DroppedObject.GetComponent<ReorderableListElement>();
            listElement.isDroppableInSpace = true;
            controller.ToggleSelectable(false);
            controller.elementIcon.overrideSprite = source.elementIcon.overrideSprite;
        }

        public void OnObjectDropped(ReorderableList.ReorderableListEventStruct e)
        {
            RectTransform droppedTransform = e.DroppedObject.transform as RectTransform;
            Canvas parentCanvas = (e.DroppedObject.transform as RectTransform).GetParentCanvas();
            Camera parentCamera = parentCanvas.worldCamera;
            Vector2 canvasSize = parentCanvas.pixelRect.size * parentCanvas.scaleFactor;
            Vector2 pivot = droppedTransform.anchorMin * droppedTransform.anchorMax;
            Vector2 screenOffset = pivot * canvasSize;
            Vector3 screenPosition = screenOffset + droppedTransform.anchoredPosition * parentCanvas.scaleFactor;
            screenPosition.z = 1;
            Ray ray = new Ray(parentCamera.ScreenToWorldPoint(screenPosition), parentCamera.transform.forward);
            Plane plane = new Plane(-parentCamera.transform.forward, Vector3.zero);
            plane.Raycast(ray, out float d);
            Vector3 worldPos = ray.GetPoint(d);

            SceneItemElementViewController controller = e.DroppedObject.GetComponent<SceneItemElementViewController>();
            if ((e.FromList.transform as RectTransform).rect.Contains(e.DroppedObject.transform.position))
            {
                Destroy(e.DroppedObject);
                return;
            }

            // create scene item
            SceneItemData data = SceneItemData.Empty;
            data.position = worldPos;
            data.payloadType = controller.sceneItemTemplateData.payloadType;
            data.payload = data.payloadType == PayloadType.Primitive
                ? controller.sceneItemTemplateData.itemName
                : controller.payload;

            ISceneItem item = m_SceneItemFactory.Create(data);

            e.DroppedObject.transform.parent = null;
            Destroy(e.DroppedObject);
        }
    }
}