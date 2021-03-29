using Aci.Unity.Models;
using Aci.Unity.Network;
using Aci.Unity.Logging;
using BotConnector.Unity;
using Aci.Unity.Events;
using Aci.Unity.Scene;
using Aci.Unity.Gamification;
using Aci.Unity.Workflow;
using UnityEngine;
using Aci.Unity.Util;
using WebSocketSharp;
using UnityEngine.Networking;
using Aci.Unity.Audio;
using System.Threading.Tasks;
using Aci.Unity.Data;
using System;
using Aci.Unity.Adaptivity;

namespace Aci.Unity.Chat
{
    public class FeedbackHandler : MonoBehaviour, IStepFinalizer
    {
        private IAciEventManager m_EventManager;
        private IAdaptivityService m_AdaptivityService;
        private IConfigProvider m_ConfigProvider;
        private IWorkflowService m_WorkflowService;
        private ITimeTrackingRepository m_TimeTrackingRepo;
        private IBadgeService m_BadgeService;
        private IAudioService m_AudioService;
        private IBot m_Bot;
        private FeedbackActivityFactory m_Factory;
        private FeedbackActivityLibrary m_ActivityLibrary;
        private AudioEventSender m_AudioSender;
        private BadgeData m_PrevBadgeData;

        [ConfigValue("assetsUrl")]
        public string assetsUrl { get; set; } = "";

        public bool finalized { get; private set; } = false;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager,
                               IAdaptivityService adaptivityService,
                               IConfigProvider configProvider,
                               IWorkflowService workflowService,
                               ITimeTrackingRepository timeTrackingRepo,
                               IBot bot,
                               IBadgeService badgeService,
                               IAudioService audioService,
                               FeedbackActivityFactory factory,
                               FeedbackActivityLibrary library)
        {
            m_ConfigProvider = configProvider;
            m_EventManager = eventManager;
            m_AdaptivityService = adaptivityService;
            m_WorkflowService = workflowService;
            m_TimeTrackingRepo = timeTrackingRepo;
            m_BadgeService = badgeService;
            m_Bot = bot;
            m_Factory = factory;
            m_ActivityLibrary = library;
            m_AudioService = audioService;

            m_ConfigProvider?.RegisterClient(this);
            //write default values to config if no config values were loaded
            if (assetsUrl.IsNullOrEmpty())
                m_ConfigProvider?.ClientDirty(this);

            RegisterForEvents();
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler<WorkflowStepEndedArgs>(this);
            m_EventManager.AddHandler<WorkflowStepFinalizedArgs>(this);
            m_WorkflowService.RegisterFinalizer(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler<WorkflowStepEndedArgs>(this);
            m_EventManager.RemoveHandler<WorkflowStepFinalizedArgs>(this);
            m_WorkflowService.UnregisterFinalizer(this);
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
            m_ConfigProvider?.UnregisterClient(this);
        }

        public void OnEvent(WorkflowStepFinalizedArgs arg)
        {
            finalized = false;
        }

        public async void OnEvent(WorkflowStepEndedArgs arg)
        {
            await new WaitForEndOfFrame();
            BadgeData gainedBadges = m_BadgeService.currentBadges - m_PrevBadgeData;
            if (arg.lastStep < 0)
            {
                m_PrevBadgeData = BadgeData.Empty;
                finalized = true;
                return;
            }
            PredefinedFeedbackActivity p = null;
            // check whether we earned badges and give feedback accordingly
            for (int i = 2; i >= 0; --i)
            {
                if(gainedBadges.TimeBadges[i] > 0)
                {
                    p = m_ActivityLibrary.Get(FeedbackType.fastBadge, i);
                    break;
                }
                if (gainedBadges.StreakBadges[i] > 0)
                {
                    p = m_ActivityLibrary.Get(FeedbackType.streakBadge, i);
                    break;
                }
                if (gainedBadges.AmountBadges[i] > 0)
                {
                    p = m_ActivityLibrary.Get(FeedbackType.amountBadge, i);
                    break;
                }
            }
            m_PrevBadgeData = m_BadgeService.currentBadges;
            if(p == null && !m_WorkflowService.currentWorkflowData.steps[arg.lastDataStep].automatic)
            {
                float expected = m_WorkflowService.currentWorkflowData.steps[arg.lastDataStep].durations[m_AdaptivityService.level];
                if (expected <= 0f)
                {
                    finalized = true;
                    return;
                }
                float actual = m_TimeTrackingRepo.currentTime.durationsPerStep[arg.lastStep];
                float diff = expected - actual;
                FeedbackType targetType = diff > 3f ? FeedbackType.assertive : diff < -3f ? FeedbackType.neutral : FeedbackType.supportive;
                p = m_ActivityLibrary.Get(targetType);
            }
            if (p == null)
            {
                finalized = true;
                return;
            }
            m_Bot.SimulateMessageReceived(m_Factory.Create(p.activity));
            string audioTrackUrl = assetsUrl + "/" + p.audioClip;
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"file:///{audioTrackUrl}", AudioType.WAV))
            {
                await www.SendWebRequest();

                if (www.isNetworkError)
                {
                    Aci.Unity.Logging.AciLog.LogError("SceneAudio", www.error);
                }
                else
                {
                    try
                    {
                        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                        await m_AudioService.PlayAudioClipAsync(clip, AudioChannels.Assistant);
                    }
                    catch(Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
            finalized = true;
        }
    }
}
