// <copyright file=ScaleToContentWidth.cs/>
// <copyright>
//   Copyright (c) 2018, Affective & Cognitive Institute
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
// <date>07/17/2018 17:29</date>

using UnityEngine;

namespace Aci.Unity.UserInterface
{
    [ExecuteInEditMode]
    public class ScaleToContentWidth : MonoBehaviour
    {
        private RectTransform _t;
        public  RectTransform content;

        public float         ExtraSpace;
        public RectTransform maxRectSize;

        private void OnEnable()
        {
            _t = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (content == null || maxRectSize == null)
                return;

            float maxWidth = Mathf.Min(content.rect.width + ExtraSpace, maxRectSize.rect.width);

            _t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
        }
    }
}