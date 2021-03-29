using Aci.Unity.Adaptivity;
using Aci.Unity.Audio;
using Aci.Unity.Events;
using Aci.Unity.Gamification;
using Aci.Unity.Util;
using Aci.Unity.Workflow;
using BotConnector.Unity;
using Microsoft.Bot.Connector.DirectLine;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Aci.Unity.Chat
{
    public class AdaptivityLevelHandler : MonoBehaviour, IStepFinalizer, IAciEventHandler<AdaptivityLevelChangeRepliedArgs>
    {
        [SerializeField]
        private AdaptivityActivityLibrary m_Library;

        private IBadgeService m_BadgeService;
        private IAciEventManager m_EventManager;
        private FeedbackActivityFactory m_Factory;
        private IBot m_Bot;
        private ITimeTrackingRepository m_TimeTracker;
        private IWorkflowService m_WorkflowService;
        private IAdaptivityService m_AdaptivityService;
        private IConfigProvider m_ConfigProvider;

        private int m_ThresholdPassedCount;
        private int m_AnswerCount = 0;

        public bool finalized { get; private set; } = false;

        /// <summary>
        /// The amount of times the user must be higher or lower than in order for the system
        /// to recommend a change in adaptivity level.
        /// </summary>
        [ConfigValue("adaptivityRepeatThreshold")]
        public float adaptivityRepeatThreshold { get; set; }

        /// <summary>
        /// The absolute duration (in seconds) that must be surpassed in order for the system 
        /// to consider a change in adaptivity level.
        /// </summary>
        [ConfigValue("adaptivityTimeThreshold")]
        public float adaptivityTimeThreshold { get; set; }

        [ConfigValue("assetsUrl")]
        public string assetsUrl { get; set; } = "";

        [Zenject.Inject]
        private void Construct(IBadgeService badgeService,
                               IAciEventManager eventManager,
                               IBot bot,
                               IWorkflowService workflowService,
                               ITimeTrackingRepository timeTracker,
                               IAdaptivityService adaptivityService,
                               FeedbackActivityFactory activityFactory,
                               IConfigProvider configProvider)
        {
            m_BadgeService = badgeService;
            m_EventManager = eventManager;
            m_Factory = activityFactory;
            m_Bot = bot;
            m_TimeTracker = timeTracker;
            m_WorkflowService = workflowService;
            m_AdaptivityService = adaptivityService;
            m_ConfigProvider = configProvider;
        }

        private void OnEnable()
        {
            m_ConfigProvider?.RegisterClient(this);
            m_WorkflowService.RegisterFinalizer(this);
            RegisterForEvents();
        }

        private void OnDisable()
        {
            m_ConfigProvider?.UnregisterClient(this);
            m_WorkflowService.UnregisterFinalizer(this);
            UnregisterFromEvents();
        }

        public void OnEvent(WorkflowStepFinalizedArgs arg)
        {
            finalized = false;
        }

        public async void OnEvent(WorkflowStepEndedArgs arg)
        {
            // Wait for badges to be awarded
            await new WaitForEndOfFrame();

            Activity activity = null;

            // Get the amount of badges gained.
            if (arg.lastStep < 0)
            {
                finalized = true;
                return;
            }

            // If the previous step was automatic or the last step, then ignore.
            if (m_WorkflowService.currentWorkflowData.steps[arg.lastDataStep].automatic)
            {
                finalized = true;
                return;
            }

            // Get the time it took to complete the last step and compare with the expected duration for the current adaptivity level.
            float expectedDuration = m_WorkflowService.currentWorkflowData.steps[arg.lastDataStep].durations[m_AdaptivityService.level];
            float currentDuration = m_TimeTracker.currentTime.durationsPerStep[arg.lastStep];
            float delta = expectedDuration - currentDuration;

            m_AnswerCount--;

            // If we're below the time threshold, then ignore.
            if(Mathf.Abs(delta) < adaptivityTimeThreshold)
            {
                finalized = true;
                return;
            }

            // Check if user was faster or slower, and update counter.
            if (delta > 0)
                m_ThresholdPassedCount++;
            else
                m_ThresholdPassedCount--;

            // If counter was surpassed then create the message.
            if (Mathf.Abs(m_ThresholdPassedCount) >= adaptivityRepeatThreshold)
            {
                // If faster, check if we've already reached the minimum adaptivity level. Reset counter if necessary.
                if (m_ThresholdPassedCount > 0)
                {
                    if (m_AdaptivityService.level > m_AdaptivityService.minLevel && m_AnswerCount < 0)
                    {
                        activity = m_Factory.Create(m_Library.DecreaseAdaptivityLevelMessage.activity);
                    }
                    m_ThresholdPassedCount = 0;
                }
                // If slower, check if we've already reached the maximum adaptivity level. Reset counter if necessary.
                else if (m_ThresholdPassedCount < 0)
                {
                    if (m_AdaptivityService.level < m_AdaptivityService.maxLevel && m_AnswerCount < 0)
                    {
                        activity = m_Factory.Create(m_Library.IncreaseAdaptivityLevelMessage.activity);
                    }
                    m_ThresholdPassedCount = 0;
                }
            }

            // If message is available, fire message otherwise set the finalized flag.
            if (activity != null)
            {
                m_Bot.SimulateMessageReceived(activity);
            }
            else
            {
                finalized = true;
            }
        }

        public void OnEvent(AdaptivityLevelChangeRepliedArgs arg)
        {
            finalized = true;

            // Reset the counter
            if (arg.wasChanged)
            {
                m_ThresholdPassedCount = 0;

            }
            m_AnswerCount = 10;
        }


        public void RegisterForEvents()
        {
            m_EventManager.AddHandler<AdaptivityLevelChangeRepliedArgs>(this);
            m_EventManager.AddHandler<WorkflowStepEndedArgs>(this);
            m_EventManager.AddHandler<WorkflowStepFinalizedArgs>(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler<AdaptivityLevelChangeRepliedArgs>(this);
            m_EventManager.RemoveHandler<WorkflowStepEndedArgs>(this);
            m_EventManager.RemoveHandler<WorkflowStepFinalizedArgs>(this);
        }
    }
}