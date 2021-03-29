using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.ContentSizeFitter;

namespace Aci.Unity.UI.Layout
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class AutoSizeLayoutElement : UIBehaviour, ILayoutElement
    {
        [SerializeField]
        private Axis m_Axis;
        [SerializeField, HideInInspector]
        private float m_MinWidth;
        [SerializeField, HideInInspector]
        private float m_PreferredWidth;
        [SerializeField, HideInInspector]
        private float m_MinHeight;
        [SerializeField, HideInInspector]
        private float m_PreferredHeight;
        [SerializeField]
        private int m_LayoutPriority;

        [SerializeField, HideInInspector]
        private List<RectTransform> m_ChildRects = new List<RectTransform>(10);
        [System.NonSerialized]
        private RectTransform m_Rect;

        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();

                return m_Rect;
            }
        }

        public float minWidth => m_MinWidth;

        public float preferredWidth => m_PreferredWidth;

        public float flexibleWidth => 0;

        public float minHeight => m_MinHeight;

        public float preferredHeight => m_PreferredHeight;

        public float flexibleHeight => 0;

        public int layoutPriority => m_LayoutPriority;

        public void CalculateLayoutInputHorizontal()
        {
            HandleSelfFittingAlongAxis(0);
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            if (axis == 0)
            {
                m_MinWidth = m_PreferredWidth = Mathf.Max(GetSizeFromChildren(axis, true), GetSizeFromChildren(axis, false));
            }
            else
            {
                m_MinHeight = m_PreferredHeight = Mathf.Max(GetSizeFromChildren(axis), GetSizeFromChildren(axis, false));
            }            
        }

        private float GetSizeFromChildren(int axis, bool preferredSize = true)
        {
            var count = m_ChildRects.Count;
            float size = 0;

            for (int i = 0; i < count; i++)
            {
                RectTransform rect = m_ChildRects[i];
                if (rect == rectTransform)
                    continue;

                size += preferredSize ? LayoutUtility.GetPreferredSize(rect, axis) : LayoutUtility.GetMinSize(rect, axis);
            }

            return size;
        }


        public void CalculateLayoutInputVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            SetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            PopulateChildren();
            SetDirty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        private void OnTransformChildrenChanged()
        {
            SetDirty();
        }

        private void Update()
        {
            SetDirty();
        }

        private void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        private void PopulateChildren()
        {
            m_ChildRects.Clear();
            GetComponentsInChildren(false, m_ChildRects);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
    }

    public enum Axis
    {
        Vertical,
        Horizontal,
        Both
    }
}

