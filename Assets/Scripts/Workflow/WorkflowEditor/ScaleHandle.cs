using Aci.Unity.Events;
using Aci.Unity.Scene;
using Aci.Unity.Workflow.WorkflowEditor;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class ScaleHandle : MonoBehaviour, ISubHandle, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    private ScaleProxyView m_ScaleProxyView;

    private SelectionHandle m_ParentHandle;

    private Vector2 m_StartPosition;
    private Vector3 m_OriginalScale;

    private bool m_IsActive = false;

    private const float m_DistanceMultiplier = 10f;

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
        handleActive.Invoke(this, enabled, enabled);
        m_ScaleProxyView.SetActive(enabled);
    }

    public void ToggleHidden(bool hidden)
    {
        gameObject.SetActive(!hidden);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ToggleActive(true, false);
        m_ScaleProxyView.SetProxy(m_ParentHandle.target.itemTransform.gameObject);
        m_StartPosition = eventData.pressPosition;
        m_OriginalScale = m_ParentHandle.target.scalable.size;
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
        Vector2 currentOffset = m_StartPosition - eventData.position;
        Vector3 newScale = m_OriginalScale;
        if (currentOffset.x < -m_DistanceMultiplier)
        {
            newScale.x *= m_DistanceMultiplier / (-currentOffset.x);
        }
        else if (currentOffset.x > m_DistanceMultiplier)
        {
            newScale.x *= currentOffset.x / m_DistanceMultiplier;
        }

        newScale.y = 1;
        if (currentOffset.y < -m_DistanceMultiplier)
        {
            newScale.y *= (-currentOffset.y) / m_DistanceMultiplier;
        }
        else if (currentOffset.y > m_DistanceMultiplier)
        {
            newScale.y *= m_DistanceMultiplier / currentOffset.y;
        }

        m_ParentHandle.target.scalable.size = newScale;
    }
}
