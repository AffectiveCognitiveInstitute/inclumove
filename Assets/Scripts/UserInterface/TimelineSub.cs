// <copyright file=TimelineSub.cs/>
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
using UnityEngine.UI.Extensions;

namespace Aci.Unity.UserInterface
{
    public class TimelineSub : MonoBehaviour
    {
        public int AddControlPointTime = 5;


        [Range(1, 10)] public float AnimationSpeed = 2f;

        private Vector2 cachedPosition;

        private float cachedWidth;

        public           int CurvePoints    = 22;
        private readonly int deformEndIndex = 14;

        private readonly int deformStartIndex = 8;

        private float          elapsedAnimationTime;
        public  UILineRenderer lineRenderer;

        public           float     MaxCurvePoints = 30;
        private          Vector2[] maxDeformDiff;
        private readonly int       maxDeformX = 20;
        private readonly int       maxDeformY = 20;
        private          float     t;


        public Vector2 Setup(float width, Vector2 position)
        {
            cachedWidth = width;
            cachedPosition = position;
            transform.localPosition = Vector3.zero;

            lineRenderer.Points = new Vector2[CurvePoints];

            lineRenderer.Points[0] = position + new Vector2(0, 0);

            //if this is an empty step set all points on same location and return
            if (width == 0)
            {
                maxDeformDiff = new Vector2[lineRenderer.Points.Length];
                maxDeformDiff[0] = Vector2.zero;
                for (int i = 1; i < lineRenderer.Points.Length - 1; i++)
                {
                    lineRenderer.Points[i] = lineRenderer.Points[0];
                    maxDeformDiff[i] = Vector2.zero;
                }

                return lineRenderer.Points[0];
            }

            for (int i = 1; i < lineRenderer.Points.Length - 1; i++)
            {
                if (i % 2 == 0)
                {
                    lineRenderer.Points[i] = lineRenderer.Points[i - 1];
                }
                else
                {
                    float factor = (float) i / (lineRenderer.Points.Length - 2);
                    lineRenderer.Points[i] = position + new Vector2(width * factor, 0);
                }
            }

            lineRenderer.Points[lineRenderer.Points.Length - 1] = position + new Vector2(width, 0);

            maxDeformDiff = new Vector2[lineRenderer.Points.Length];
            for (int i = deformStartIndex; i <= deformEndIndex; i++)
                maxDeformDiff[i] = lineRenderer.Points[i] +
                                   new Vector2(maxDeformX - Random.value * 5, maxDeformY - Random.value * 5);

            return lineRenderer.Points[lineRenderer.Points.Length - 1];
        }

        public void SetColor(Color sublineColor)
        {
            lineRenderer.color = sublineColor;
        }

        public void Deform(float totalLength, float maxDeform)
        {
            if (lineRenderer.Points.Length <= MaxCurvePoints)
            {
                elapsedAnimationTime += Time.deltaTime;
                if (elapsedAnimationTime > AddControlPointTime)
                {
                    setupNewCurvePoints(lineRenderer.Points.Length + 1);
                    elapsedAnimationTime = 0;
                }
            }

            // check if current line at max
            // 
            // if true add point
            // grow line for missing percentage
            t += Time.deltaTime * AnimationSpeed;
            for (int i = deformStartIndex; i <= lineRenderer.Points.Length - deformStartIndex; i++)
            {
                lineRenderer.Points[i].y = maxDeformDiff[i].y + oscillate(t, i * 0.1f * AnimationSpeed, maxDeformY);
                lineRenderer.Points[i].x = maxDeformDiff[i].x +
                                           oscillate(t, (deformEndIndex - i + 1) * 0.1f * AnimationSpeed, maxDeformX);
            }


            //Update curve
            lineRenderer.SetAllDirty();
        }

        private void setupNewCurvePoints(int segmentAmount)
        {
            Debug.LogWarning("CACHED POS: " + cachedPosition);
            Vector2[] oldControlProints = lineRenderer.Points;
            lineRenderer.Points = new Vector2[segmentAmount];
            int additionalPointIndex = Mathf.FloorToInt(lineRenderer.Points.Length * 0.5f);


            for (int i = 1; i < lineRenderer.Points.Length - 1; i++)
                if (i % 2 == 0)
                {
                    lineRenderer.Points[i] = lineRenderer.Points[i - 1];
                }
                else
                {
                    float factor = (float) i / (lineRenderer.Points.Length - 2);
                    lineRenderer.Points[i] = cachedPosition + new Vector2(cachedWidth * factor, 0);

                    //Debug.Log("New Pos: " + (cachedPosition.x + cachedWidth * factor));
                }

            lineRenderer.Points[lineRenderer.Points.Length - 1] = oldControlProints[oldControlProints.Length - 1];

            maxDeformDiff = new Vector2[lineRenderer.Points.Length];
            for (int i = deformStartIndex; i <= lineRenderer.Points.Length - deformStartIndex; i++)
                maxDeformDiff[i] = lineRenderer.Points[i] +
                                   new Vector2(maxDeformX - Random.value * 5, maxDeformY - Random.value * 5);
        }

        private float oscillate(float time, float speed, float scale, bool useSin = false)
        {
            return Mathf.Cos(time * speed / Mathf.PI) * scale;
        }
    }
}