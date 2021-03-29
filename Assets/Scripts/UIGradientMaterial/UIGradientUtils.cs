// <copyright file=UIGradientUtils.cs/>
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

public static class UIGradientUtils
{
    public static Vector2[] VerticePositions { get; } = {Vector2.up, Vector2.one, Vector2.right, Vector2.zero};

    public static Matrix2x3 LocalPositionMatrix(Rect rect, Vector2 dir)
    {
        float cos = dir.x;
        float sin = dir.y;
        Vector2 rectMin = rect.min;
        Vector2 rectSize = rect.size;
        float c = 0.5f;
        float ax = rectMin.x / rectSize.x + c;
        float ay = rectMin.y / rectSize.y + c;
        float m00 = cos / rectSize.x;
        float m01 = sin / rectSize.y;
        float m02 = -(ax * cos - ay * sin - c);
        float m10 = sin / rectSize.x;
        float m11 = cos / rectSize.y;
        float m12 = -(ax * sin + ay * cos - c);
        return new Matrix2x3(m00, m01, m02, m10, m11, m12);
    }

    public static Vector2 RotationDir(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);
        return new Vector2(cos, sin);
    }

    public static Vector2 CompensateAspectRatio(Rect rect, Vector2 dir)
    {
        float ratio = rect.height / rect.width;
        dir.x *= ratio;
        return dir.normalized;
    }

    public static float InverseLerp(float a, float b, float v)
    {
        return a != b ? (v - a) / (b - a) : 0f;
    }

    public static Color Bilerp(Color a1, Color a2, Color b1, Color b2, Vector2 t)
    {
        Color a = Color.LerpUnclamped(a1, a2, t.x);
        Color b = Color.LerpUnclamped(b1, b2, t.x);
        return Color.LerpUnclamped(a, b, t.y);
    }

    public static void Lerp(UIVertex a, UIVertex b, float t, ref UIVertex c)
    {
        c.position = Vector3.LerpUnclamped(a.position, b.position, t);
        c.normal = Vector3.LerpUnclamped(a.normal, b.normal, t);
        c.color = Color32.LerpUnclamped(a.color, b.color, t);
        c.tangent = Vector3.LerpUnclamped(a.tangent, b.tangent, t);
        c.uv0 = Vector3.LerpUnclamped(a.uv0, b.uv0, t);
        c.uv1 = Vector3.LerpUnclamped(a.uv1, b.uv1, t);
        // c.uv2 = Vector3.LerpUnclamped(a.uv2, b.uv2, t);
        // c.uv3 = Vector3.LerpUnclamped(a.uv3, b.uv3, t);		
    }

    public struct Matrix2x3
    {
        public float m00, m01, m02, m10, m11, m12;

        public Matrix2x3(float m00, float m01, float m02, float m10, float m11, float m12)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;
            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;
        }

        public static Vector2 operator *(Matrix2x3 m, Vector2 v)
        {
            float x = m.m00 * v.x - m.m01 * v.y + m.m02;
            float y = m.m10 * v.x + m.m11 * v.y + m.m12;
            return new Vector2(x, y);
        }
    }
}