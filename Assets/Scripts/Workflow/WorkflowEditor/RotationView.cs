// <copyright file=RotationView.cs/>
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
// <date>10/24/2019 10:07</date>

using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class RotationView : MonoBehaviour
    {
        [SerializeField]
        private UICircle m_RotationArc;

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        private bool m_IsActive = false;

        public void SetActive(bool active)
        {
            if (m_IsActive == active)
                return;
            m_IsActive = active;
            m_CanvasGroup.alpha = active ? 1 : 0;
        }

        public void SetRotation(float radAngle)
        {
            m_RotationArc.ArcInvert = radAngle < 0;
            m_RotationArc.SetProgress(Mathf.Abs(radAngle));
        }
    }
}