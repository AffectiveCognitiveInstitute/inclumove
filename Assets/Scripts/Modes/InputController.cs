using Aci.Unity.Events;
using Aci.Unity.Models;
using Aci.Unity.Network;
using Aci.Unity.Scene;
using System.Collections;
using System.Collections.Generic;
using Aci.Unity.Workflow;
using UnityEngine;

public class InputController : MonoBehaviour
                             , IAciEventHandler<WorkflowStartArgs>
                             , IAciEventHandler<WorkflowStopArgs>
{
    IAciEventManager m_EventManager;
    INetworkPublisher m_Publisher;
    IWorkflowService m_WorkflowService;

    private bool m_WorkflowStarted = false;
    private float m_LastTime = 0;

    [Zenject.Inject]
    private void Construct(IAciEventManager eventManager
                         , INetworkPublisher publisher
                         , IWorkflowService workflowService)
    {
        m_EventManager = eventManager;
        m_Publisher = publisher;
        m_WorkflowService = workflowService;

        RegisterForEvents();
    }

    public void Update()
    {
        if (Time.time - m_LastTime < 1f)
            return;
        
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Escape))
        {
            m_EventManager?.Invoke(new UserDetectedArgs());
            m_LastTime = Time.time;
        }

        if (!m_WorkflowStarted)
            return;

        if (Input.GetKeyDown(KeyCode.Space) || (Input.GetKeyDown(KeyCode.Mouse0) && Input.mousePosition.x > 500))
        {
            m_WorkflowService.SetStep(m_WorkflowService.currentStep + 1);
            m_LastTime = Time.time;
        }
        
        if (Input.GetKeyDown(KeyCode.Return) || (Input.GetKeyDown(KeyCode.Mouse1) && Input.mousePosition.x > 500))
        {
            m_WorkflowService.SetStep(m_WorkflowService.currentStep - 1);
            m_LastTime = Time.time;
        }
    }

    public void RegisterForEvents()
    {
        m_EventManager.AddHandler<WorkflowStartArgs>(this);
        m_EventManager.AddHandler<WorkflowStopArgs>(this);
    }

    public void UnregisterFromEvents()
    {
        m_EventManager.RemoveHandler<WorkflowStartArgs>(this);
        m_EventManager.RemoveHandler<WorkflowStopArgs>(this);
    }

    public void OnEvent(WorkflowStartArgs arg)
    {
        m_WorkflowStarted = true;
    }

    public void OnEvent(WorkflowStopArgs arg)
    {
        m_WorkflowStarted = false;
    }
}
