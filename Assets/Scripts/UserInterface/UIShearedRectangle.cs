using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Aci.Unity.UserInterface
{
    [AddComponentMenu("UI/Extensions/Primitives/UI Sheared Rectangle")]
    public class UIShearedRectangle : UIPrimitiveBase
    {
        public bool fill = true;
        public float thickness = 5;
        [Range(-89, 89)] 
        public float theta = 0;
        private float size = 0;

        void Update()
        {
            size = rectTransform.rect.width;
            if (rectTransform.rect.width > rectTransform.rect.height)
                size = rectTransform.rect.height;
            else
                size = rectTransform.rect.width;
            thickness = (float)Mathf.Clamp(thickness, 0, size / 2);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Vector2 dim = rectTransform.rect.size;
            
            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;
            Vector2 uv0 = new Vector2(0, 0);
            Vector2 uv1 = new Vector2(0, 1);
            Vector2 uv2 = new Vector2(1, 1);
            Vector2 uv3 = new Vector2(1, 0);
            
            Vector2 shearVector = new Vector2(rectTransform.rect.height / Mathf.Tan(Mathf.Deg2Rad * (90 - Mathf.Abs(theta))), 0);

            Vector2[] pos =
            {
                (uv0 - Vector2.one * 0.5f) * dim,
                (uv1 - Vector2.one * 0.5f) * dim,
                (uv2 - Vector2.one * 0.5f) * dim,
                (uv3 - Vector2.one * 0.5f) * dim
            };
            if (theta > 0)
            {
                pos[1] = (uv1 - Vector2.one * 0.5f) * dim + shearVector;
                pos[3] = (uv3 - Vector2.one * 0.5f) * dim - shearVector;
            }
            else if (theta < 0)
            {
                pos[0] = (uv0 - Vector2.one * 0.5f) * dim + shearVector;
                pos[2] = (uv2 - Vector2.one * 0.5f) * dim - shearVector;
            }

            Vector2 pos0;
            Vector2 pos1;
            Vector2 pos2;
            Vector2 pos3;
            for (int i = 0; i < 4; ++i)
            {
                pos0 = pos[i];
                pos1 = pos[(i + 1) % 4];
                if (fill)
                {
                    pos2 = pos[(i + 1) % 4] - (pos[(i + 1) % 4] - pos[(i + 2) % 4]) * 0.5f;
                    pos3 = pos[i] - (pos[i] - pos[(i + 3) % 4]) * 0.5f;
                }
                else
                {
                    pos2 = pos[(i + 1) % 4] - (pos[(i + 1) % 4] - pos[(i + 2) % 4]).normalized * thickness;
                    pos3 = pos[i] - (pos[i] - pos[(i + 3) % 4]).normalized * thickness;
                }
                vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
            }
        }
    }
}