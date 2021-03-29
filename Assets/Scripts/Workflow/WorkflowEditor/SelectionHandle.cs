using System.Collections.Generic;
using Aci.Unity.Events;
using Aci.Unity.Scene;
using Aci.Unity.Scene.SceneItems;
using Aci.Unity.Workflow.WorkflowEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI.Extensions;

public class SelectionHandle : MonoBehaviour, IAciEventHandler<SelectionChanged>
{
    private IAciEventManager m_Broker;

    private List<ISubHandle> m_Handles = new List<ISubHandle>();

    [SerializeField]
    private float m_MinWidth = 200f;

    [SerializeField]
    private float m_HandleOffset = 0f;

    public class HandleActiveEvent : UnityEvent<ISubHandle, bool, bool>
    {
    }

    public ISceneItem target { get; private set; }

    [Zenject.Inject]
    private void Construct(IAciEventManager broker)
    {
        m_Broker = broker;
        RegisterForEvents();
        foreach (ISubHandle handle in GetComponentsInChildren<ISubHandle>())
        {
            m_Handles.Add(handle);
        }
    }

    public void OnDestroy()
    {
        UnregisterFromEvents();
    }

    public void Update()
    {
        Vector2 size = (gameObject.transform as RectTransform).sizeDelta;
        Vector2 targetSize = target.scalable.size * 100;
        targetSize += Vector2.one * m_HandleOffset;
        size.x = size.y = Mathf.Max(Mathf.Max(targetSize.x, targetSize.y), m_MinWidth);
        (gameObject.transform as RectTransform).sizeDelta = size;

        Canvas parentCanvas = (gameObject.transform as RectTransform).GetParentCanvas();
        Camera parentCamera = parentCanvas.worldCamera;
        Vector2 screenPosition = parentCamera.WorldToScreenPoint(target.itemTransform.position);
        (gameObject.transform as RectTransform).anchoredPosition = screenPosition / parentCanvas.scaleFactor;
    }

    public void RegisterForEvents()
    {
        m_Broker.AddHandler(this);
    }

    public void UnregisterFromEvents()
    {
        m_Broker.RemoveHandler(this);
    }

    public void OnEvent(SelectionChanged arg)
    {
        foreach (ISubHandle curHandle in m_Handles)
        {
            curHandle.ToggleActive(false, true);
        }
        if (arg.current == null)
        {
            target = null;
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        target = (arg.current as Draggable).GetComponent<ISceneItem>();

        Vector2 size = (gameObject.transform as RectTransform).sizeDelta;
        size.x = target.itemTransform.localScale.x * 100;
        (gameObject.transform as RectTransform).sizeDelta = size;

        Canvas parentCanvas = (gameObject.transform as RectTransform).GetParentCanvas();
        Camera parentCamera = parentCanvas.worldCamera;
        Vector2 screenPosition = parentCamera.WorldToScreenPoint(target.itemTransform.position);
        (gameObject.transform as RectTransform).anchoredPosition = screenPosition / parentCanvas.scaleFactor;
    }

    public void OnHandleActivated(ISubHandle handle, bool active, bool hide)
    {
        foreach(ISubHandle curHandle in m_Handles)
        {
            if (curHandle == handle)
                continue;
            curHandle.ToggleHidden(hide);
            if(active)
                curHandle.ToggleActive(false, true);
        }
    }
}
