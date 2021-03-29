using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MaskableGraphic), typeof(RectTransform))]
    public class HideIfOverlapsRectTransform : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_TargetRectTransform;

        private MaskableGraphic m_Graphic;
        private RectTransform m_RectTransform;

        private RectTransform rectTransform
        {
            get 
            {
                if (m_RectTransform == null)
                    m_RectTransform = GetComponent<RectTransform>();

                return m_RectTransform;
            }
        }

        private MaskableGraphic graphic
        {
            get
            {
                if (m_Graphic == null)
                    m_Graphic = GetComponent<MaskableGraphic>();

                return m_Graphic;
            }
        }

        private void Update()
        {
            if(m_TargetRectTransform == null)
            {
                graphic.enabled = true;
                return;
            }

            bool isOverlapping = IsOverlapping(rectTransform, m_TargetRectTransform);
            graphic.enabled = !isOverlapping;
        }

        private bool IsOverlapping(RectTransform a, RectTransform b)
        {
            Rect rectA = ToWorldSpaceRect(a);
            Rect rectB = ToWorldSpaceRect(b);

            return rectA.Overlaps(rectB);
        }

        public static Rect ToWorldSpaceRect(RectTransform r)
        {
            Rect rect = r.rect;
            rect.center = r.TransformPoint(rect.center);
            rect.size = r.TransformVector(rect.size);
            return rect;
        }
    }
}

