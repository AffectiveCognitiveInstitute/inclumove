// <copyright file=DuplicateHandle.cs/>
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
// <date>10/24/2019 14:11</date>

using Aci.Unity.Data.JsonModel;
using Aci.Unity.Scene.SceneItems;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class DuplicateHandle : MonoBehaviour, ISubHandle
    {
        private SceneItemData.Factory m_DataFactory;
        private SceneItem.Factory     m_ItemFactory;

        private SelectionHandle m_ParentHandle;

        public SelectionHandle.HandleActiveEvent handleActive { get; } = new SelectionHandle.HandleActiveEvent();

        public void ToggleActive(bool enabled, bool silent)
        {
            if (silent)
                return;
            handleActive.Invoke(this, enabled, false);
        }

        public void ToggleHidden(bool hidden)
        {
            gameObject.SetActive(!hidden);
        }

        private void Awake()
        {
            m_ParentHandle = transform.parent.GetComponent<SelectionHandle>();
            handleActive.AddListener(m_ParentHandle.OnHandleActivated);
        }

        [Inject]
        private void Construct(SceneItem.Factory itemFactory, SceneItemData.Factory dataFactory)
        {
            m_ItemFactory = itemFactory;
            m_DataFactory = dataFactory;
        }

        public void DuplicateTarget()
        {
            SceneItemData data = m_DataFactory.Create(m_ParentHandle.target);
            m_ItemFactory.Create(data);
        }
    }
}