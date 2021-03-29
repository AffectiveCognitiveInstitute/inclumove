// <copyright file=ColorHandle.cs/>
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

using Aci.Unity.Workflow.WorkflowEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class ColorHandle : MonoBehaviour, ISubHandle
    {
        [SerializeField] private RectTransform m_ColorSelectorTransform;

        private bool            m_IsActive;
        private SelectionHandle m_ParentHandle;

        public SelectionHandle.HandleActiveEvent handleActive { get; } = new SelectionHandle.HandleActiveEvent();

        public void ToggleActive(bool enabled, bool silent)
        {
            m_IsActive = enabled;
            m_ColorSelectorTransform.gameObject.SetActive(enabled);
            if (silent)
                return;
            handleActive.Invoke(this, enabled, false);
        }

        public void ToggleHidden(bool hidden)
        {
            gameObject.SetActive(!hidden);
            m_ColorSelectorTransform.gameObject.SetActive(m_IsActive && !hidden);
        }

        private void Awake()
        {
            m_ParentHandle = transform.parent.GetComponent<SelectionHandle>();
            handleActive.AddListener(m_ParentHandle.OnHandleActivated);
            m_ColorSelectorTransform.gameObject.SetActive(false);
        }

        public void ToggleActive()
        {
            ToggleActive(!m_IsActive, false);
        }

        public void OnColorButtonPressed(Image source)
        {
            m_ParentHandle.target.colorable.color = source.color;
        }
    }
}