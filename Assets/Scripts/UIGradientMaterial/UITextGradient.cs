// <copyright file=UITextGradient.cs/>
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
// <date>07/12/2018 05:59</date>

using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Text Gradient")]
public class UITextGradient : BaseMeshEffect
{
    [Range(-180f, 180f)] public float m_angle = 0f;

    public Color m_color1 = Color.white;
    public Color m_color2 = Color.white;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (enabled)
        {
            Rect rect = graphic.rectTransform.rect;
            Vector2 dir = UIGradientUtils.RotationDir(m_angle);
            UIGradientUtils.Matrix2x3 localPositionMatrix =
                UIGradientUtils.LocalPositionMatrix(new Rect(0f, 0f, 1f, 1f), dir);

            UIVertex vertex = default(UIVertex);
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                Vector2 position = UIGradientUtils.VerticePositions[i % 4];
                Vector2 localPosition = localPositionMatrix * position;
                vertex.color *= Color.Lerp(m_color2, m_color1, localPosition.y);
                vh.SetUIVertex(vertex, i);
            }
        }
    }
}