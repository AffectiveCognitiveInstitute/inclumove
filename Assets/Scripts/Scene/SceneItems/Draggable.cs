using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Aci.Unity.Scene.SceneItems
{
    class Draggable : MonoBehaviour, ISelectable
    {
        private class SelectionEvent : UnityEvent<ISelectable> { }

        private ISelectionService m_SelectionService;

        public UnityEvent<ISelectable> selectionChanged { get; } = new Draggable.SelectionEvent();

        private bool m_IsSelected = false;

        private float m_ZOffset;
        private Vector3 m_ClickOffset;
        private float m_TotalMovement;
        private float m_MovementThreshold;

        private bool m_Dragging = false;

        public bool isSelected
        {
            get => m_IsSelected;
            set
            {
                if (m_IsSelected == value)
                    return;
                m_IsSelected = value;
                selectionChanged?.Invoke(this);
            }
        }

        [Zenject.Inject]
        private void Construct([Zenject.InjectOptional]ISelectionService selectionService)
        {
            m_SelectionService = selectionService;
            if (m_SelectionService == null)
                enabled = false;
        }

        private void OnEnable()
        {
            m_SelectionService?.Register(this);
        }

        private void OnDisable()
        {
            isSelected = false;
            m_SelectionService?.Unregister(this);
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            m_Dragging = true;
            m_MovementThreshold = EventSystem.current.pixelDragThreshold;
            m_TotalMovement = 0;
            m_ZOffset = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            m_ClickOffset = gameObject.transform.position - GetWorldMousePosition(m_ZOffset);
        }

        private void OnMouseDrag()
        {
            if (!m_Dragging)
                return;
            Vector3 newMousePos = GetWorldMousePosition(m_ZOffset);
            Vector3 newPos = newMousePos + m_ClickOffset;
            m_TotalMovement += (gameObject.transform.position - newPos).magnitude;
            if (m_TotalMovement > m_MovementThreshold)
            {
                gameObject.transform.position = newPos;
            }
        }

        private void OnMouseUp()
        {
            m_Dragging = false;
            if (m_TotalMovement >= m_MovementThreshold || EventSystem.current.IsPointerOverGameObject())
                return;
            isSelected = true;
        }

        private Vector3 GetWorldMousePosition(float camDistance)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = camDistance;
            return Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }
}
