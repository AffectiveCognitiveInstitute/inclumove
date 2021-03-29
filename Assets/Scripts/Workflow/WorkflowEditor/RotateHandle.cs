using Aci.Unity.Events;
using Aci.Unity.Scene;
using Aci.Unity.Workflow.WorkflowEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class RotateHandle : MonoBehaviour, ISubHandle, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    private RotationView m_RotationView;

    private SelectionHandle m_ParentHandle;

    private Vector2 m_StartPosition;
    private float m_OriginalRotation;

    private bool m_IsActive = false;

    void Awake()
    {
        m_ParentHandle = transform.parent.GetComponent<SelectionHandle>();
        handleActive.AddListener(m_ParentHandle.OnHandleActivated);
    }

    public SelectionHandle.HandleActiveEvent handleActive { get; } = new SelectionHandle.HandleActiveEvent();

    public void ToggleActive(bool enabled, bool silent)
    {
        m_IsActive = enabled;
        if (silent)
            return;
        m_RotationView.SetActive(enabled);
        m_RotationView.SetRotation(0);
        handleActive.Invoke(this, enabled, enabled);
    }

    public void ToggleHidden(bool hidden)
    {
        gameObject.SetActive(!hidden);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ToggleActive(true, false);
        m_StartPosition = (m_ParentHandle.transform as RectTransform).anchoredPosition;
        m_OriginalRotation = m_ParentHandle.target.itemTransform.rotation.eulerAngles.z;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!m_IsActive)
            return;
        ToggleActive(false, false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!m_IsActive)
            return;
        Vector2 currentDirection = (eventData.position - m_StartPosition).normalized;

        float forwardOrientation = Vector2.Dot(Vector2.up, currentDirection) + 1;
        float rightOrientation = Vector2.Dot(Vector2.right, currentDirection) * -1;

        float halfAngle = Mathf.Sign(rightOrientation) * (1 - (forwardOrientation * 0.5f));

        m_RotationView.SetRotation(halfAngle * 0.5f);

        m_ParentHandle.target.itemTransform.rotation = Quaternion.Euler(0, 0, m_OriginalRotation + (180 * halfAngle));
    }
}
