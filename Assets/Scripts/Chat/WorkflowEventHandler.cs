using Aci.Unity.Bot;
using Aci.Unity.Events;
using Aci.Unity.Scene;
using Aci.Unity.UI.Localization;
using Aci.Unity.UserInterface;
using BotConnector.Unity;
using Microsoft.Bot.Connector.DirectLine;
using System;
using Aci.Unity.Workflow;
using UnityEngine;

namespace Aci.Unity.Chat
{
    /// <summary>
    /// Class responsible for receiving events from core and notifies bots
    /// about changes regarding the workflow.
    /// </summary>
    public class WorkflowEventHandler : MonoBehaviour,
                                        IAciEventHandler<WorkflowLoadArgs>,
                                        IAciEventHandler<WorkflowStartArgs>,
                                        IAciEventHandler<WorkflowStepFinalizedArgs>,
                                        IAciEventHandler<WorkflowStopArgs>,
                                        IAciEventHandler<LocalizationChangedArgs>
    {

        private const string ClientFinishedWorkflowStep = "clientFinishedWorkflowStep";
        private const string ClientReadyForWorkflow = "clientReadyForWorkflow";

        private IAciEventManager m_EventManager;
        private IBotMessenger m_BotMessenger;
        private ILocalizationManager m_LocalizationManager;
        private IWorkflowService m_WorkflowService;
        private IChatWindowFacade m_ChatWindowFacade;
        private int m_LastWorkStep = -1;
        private string m_Locale = "de";

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager,
                               IBotMessenger botMessenger,
                               ILocalizationManager localizationManager,
                               IWorkflowService workflowService,
                               IChatWindowFacade chatWindowFacade)
        {
            m_EventManager = eventManager;
            m_BotMessenger = botMessenger;
            m_LocalizationManager = localizationManager;
            m_WorkflowService = workflowService;
            m_ChatWindowFacade = chatWindowFacade;
        }

        private void Awake()
        {
            m_Locale = m_LocalizationManager.currentLocalization;
        }

        private void OnEnable()
        {
            RegisterForEvents();
        }

        private void OnDisable()
        {
            UnregisterFromEvents();
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler<WorkflowLoadArgs>(this);
            m_EventManager.AddHandler<WorkflowStepFinalizedArgs>(this);
            m_EventManager.AddHandler<WorkflowStopArgs>(this);
            m_EventManager.AddHandler<WorkflowStartArgs>(this);
            m_EventManager.AddHandler<LocalizationChangedArgs>(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler<WorkflowLoadArgs>(this);
            m_EventManager.RemoveHandler<WorkflowStepFinalizedArgs>(this);
            m_EventManager.RemoveHandler<WorkflowStopArgs>(this);
            m_EventManager.RemoveHandler<WorkflowStartArgs>(this);
            m_EventManager.RemoveHandler<LocalizationChangedArgs>(this);
        }

        void IAciEventHandler<WorkflowLoadArgs>.OnEvent(WorkflowLoadArgs arg)
        {
            try
            {
                m_BotMessenger.SendEventAsync(ClientReadyForWorkflow, arg.msg);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void IAciEventHandler<WorkflowStepFinalizedArgs>.OnEvent(WorkflowStepFinalizedArgs arg)
        {
            if (m_LastWorkStep == arg.newStep)
                return; 

            m_LastWorkStep = arg.newStep;

            if (m_LastWorkStep == 0)
                return;

            m_BotMessenger.SendEventAsync(ClientFinishedWorkflowStep, m_WorkflowService.currentWorkflowData.steps[arg.lastDataStep].id);
        }

        void IAciEventHandler<WorkflowStopArgs>.OnEvent(WorkflowStopArgs arg)
        {
        }

        void IAciEventHandler<WorkflowStartArgs>.OnEvent(WorkflowStartArgs arg)
        {
            // TODO: this is currently not used in the IFA-demonstrator since the intermediate json of the bot doesnt discern between workflow loading and starting, FIX THIS AFTER!
            // SendMessageToBot("event", workflowStart, "workflow_started", false);
            m_LastWorkStep = -1;

            // Clear the chat
            m_ChatWindowFacade.Clear();
        }

        void IAciEventHandler<LocalizationChangedArgs>.OnEvent(LocalizationChangedArgs arg)
        {
            m_Locale = arg.ietf;
        }
    }
}
