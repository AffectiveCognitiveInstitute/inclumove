using System.Collections.Generic;
using UnityEngine;
using Aci.Unity.Workflow;
using Aci.Unity.Events;
using Aci.Unity.UserInterface.Animation;
using Aci.Unity.UserInterface.ViewControllers;
using Aci.Unity.Data;
using Aci.Unity.Data.JsonModel;

namespace Aci.Unity.Gamification
{
    public class TopBarFinalizer : MonoBehaviour
                                 , IStepFinalizer
                                 , IAciEventHandler<WorkflowLoadArgs>
    {
        private IAciEventManager m_EventManager;
        private IWorkflowService m_WorkflowService;
        private IBadgeService m_BadgeService;

        [SerializeField]
        private GamificationTimeline m_GamificationTimeline;

        [SerializeField]
        private BadgeOverviewViewController m_BadgeOverviewController;

        [SerializeField]
        private AssemblyProgressViewController m_AssemblyProgressViewController;

        [SerializeField]
        private AssemblyCounterViewController m_AssemblyCounterViewController;

        [SerializeField]
        private QueueableParticleAnimation m_WaveBubbles;

        [SerializeField]
        private QueueableParticleAnimation m_AssemblyProgressBubbles;

        private IAnimationQueue m_AnimationQueue = new AnimationQueue();

        private BadgeData m_PrevBadgeData = BadgeData.Empty;
        private int m_PrevRepetitions = 0;
        private List<int> m_RepetitionSteps = new List<int>();

        public bool finalized { get; private set; } = false;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager, IWorkflowService workflowService, IBadgeService badgeService)
        {
            m_EventManager = eventManager;
            m_WorkflowService = workflowService;
            m_BadgeService = badgeService;
            RegisterForEvents();
        }

        public void OnEvent(WorkflowLoadArgs arg)
        {
            m_RepetitionSteps.Clear();
            WorkflowStepData[] data = m_WorkflowService.currentWorkflowData.steps;
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i].repetitions == 0)
                    continue;
                m_RepetitionSteps.Add(i);
            }
        }

        public void OnEvent(WorkflowStepFinalizedArgs arg)
        {
            finalized = false;
            m_PrevRepetitions = arg.executedRepetitions;
        }

        public async void OnEvent(WorkflowStepEndedArgs arg)
        {
            //wait until all other end events have been handled
            await new WaitForEndOfFrame();
            // queue counter background animation
            int index = m_RepetitionSteps.IndexOf(arg.lastDataStep);
            if(index != -1)
            {
                float fill = (float)(index+1) / (float)m_RepetitionSteps.Count;
                QueueableAnimationSynchronizer sync = QueueableAnimationSynchronizer.Pool.Spawn();
                m_AssemblyProgressViewController.SetTarget(fill);
                sync.Append(m_GamificationTimeline.GetItemProgressAnimation(arg.lastDataStep), m_AssemblyProgressViewController);
                m_AnimationQueue.Enqueue(sync);
            }
            // queue repetition finished animation
            if (m_PrevRepetitions < arg.executedRepetitions)
            {
                QueueableAnimationSynchronizer sync = QueueableAnimationSynchronizer.Pool.Spawn();
                sync.Append(m_WaveBubbles, m_AssemblyProgressBubbles);
                m_AnimationQueue.Enqueue(sync);

                sync = QueueableAnimationSynchronizer.Pool.Spawn();
                m_AssemblyCounterViewController.SetTarget(arg.executedRepetitions);
                sync.Append(m_GamificationTimeline, m_AssemblyCounterViewController);
                m_AnimationQueue.Enqueue(sync);
            }
            // queue badge animations
            BadgeData changedBadges = m_BadgeService.currentBadges - m_PrevBadgeData;
            if (changedBadges != BadgeData.Empty)
            {
                m_PrevBadgeData = m_BadgeService.currentBadges;
                m_BadgeOverviewController.SetBadgeData(changedBadges);
                m_AnimationQueue.Enqueue(m_BadgeOverviewController);
            }
            await m_AnimationQueue.Play();
            finalized = true;
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler<WorkflowStepEndedArgs>(this);
            m_EventManager.AddHandler<WorkflowStepFinalizedArgs>(this);
            m_EventManager.AddHandler<WorkflowLoadArgs>(this);
            m_WorkflowService.RegisterFinalizer(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler<WorkflowStepEndedArgs>(this);
            m_EventManager.RemoveHandler<WorkflowStepFinalizedArgs>(this);
            m_EventManager.RemoveHandler<WorkflowLoadArgs>(this);
            m_WorkflowService.UnregisterFinalizer(this);
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

    }
}
