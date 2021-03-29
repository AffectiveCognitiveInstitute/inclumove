using System;
using UnityEngine;
using Zenject;
using Aci.Unity.Data;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Events;
using Aci.Unity.Workflow;
using System.Collections.Generic;
using Aci.Unity.Util;
using Unity.Mathematics;

namespace Aci.Unity.Gamification
{
    public class BadgeService : IInitializable
                              , IDisposable
                              , IBadgeService
                              , IAciEventHandler<WorkflowStepEndedArgs>
                              , IAciEventHandler<WorkflowLoadArgs>
    {
        private IAciEventManager m_EventManager;
        private IUserManager m_UserManager;
        private IWorkflowService m_WorkflowService;
        private ITimeProvider m_TimeProvider;
        private ITimeTrackingRepository m_TimeRepo;


        private int[] m_AmountLevels = { 0, 0, 0 };
        private int[] m_FastLevels = { 0, 0, 0 };
        private int[] m_StreakLevels = { 3, 5, 8 };

        private int m_StreakCounter = 0;
        private int m_FastCounter = 0;
        private int m_PrevRepetitions = 0;

        private BadgeData m_CurrentBadges = BadgeData.Empty;
        public BadgeData currentBadges => m_CurrentBadges;

        public IReadOnlyList<int> fastLevels => Array.AsReadOnly<int>(m_FastLevels);

        public IReadOnlyList<int> streakLevels => Array.AsReadOnly<int>(m_StreakLevels);

        public IReadOnlyList<int> amountLevels => Array.AsReadOnly<int>(m_AmountLevels);

        public int currentFast => m_FastCounter;

        public int currentStreak => m_StreakCounter;

        public int currentAmount => m_PrevRepetitions;

        public BadgeService(IAciEventManager eventManager
                          , IUserManager userManager
                          , IWorkflowService workflowService
                          , ITimeProvider timeProvider
                          , ITimeTrackingRepository timeRepo)
        {
            m_EventManager = eventManager;
            m_UserManager = userManager;
            m_WorkflowService = workflowService;
            m_TimeProvider = timeProvider;
            m_TimeRepo = timeRepo;
        }

        public void Initialize()
        {
            RegisterForEvents();
        }

        public void Dispose()
        {
            UnregisterFromEvents();
        }

        public void OnEvent(WorkflowLoadArgs arg)
        {
            m_CurrentBadges = BadgeData.Empty;
            int repetitionAmount = 0;
            int stepAmount = 0;
            foreach(WorkflowStepData stepData in m_WorkflowService.currentWorkflowData.steps)
            {
                repetitionAmount = stepData.repetitions < repetitionAmount ? repetitionAmount : stepData.repetitions;
                if (stepData.repetitions > 0 && !stepData.automatic)
                    ++stepAmount;
            }
            ++repetitionAmount;
            m_AmountLevels = new int[] { Mathf.FloorToInt(repetitionAmount * 0.5f),
                                         Mathf.FloorToInt(repetitionAmount * 0.75f),
                                         repetitionAmount };
            m_FastLevels = new int[] { 0, Mathf.FloorToInt(stepAmount * 0.5f), stepAmount };
            //TODO: move this to a workflow configuration
            m_StreakLevels = new int[] { Mathf.FloorToInt(stepAmount * repetitionAmount * 0.5f),
                                       Mathf.FloorToInt(stepAmount * repetitionAmount * 0.75f),
                                       stepAmount * repetitionAmount };

            m_StreakCounter = 0;
            m_FastCounter = 0;
            m_PrevRepetitions = 0;
        }

        public void OnEvent(WorkflowStepEndedArgs arg)
        {
            int3 amountBadges = EvaluateAmountBadge(arg);
            int3 fastBadges = EvaluateFastBadge(arg);
            int3 streakBadges = EvaluateStreakBadge(arg);
            bool badgeAmountHasChanged = math.any(amountBadges > 0) ||
                                    math.any(fastBadges > 0) ||
                                    math.any(streakBadges > 0);
            m_CurrentBadges.AmountBadges += amountBadges;
            m_CurrentBadges.TimeBadges += fastBadges;
            m_CurrentBadges.StreakBadges += streakBadges;
            m_PrevRepetitions = arg.executedRepetitions;

            if (badgeAmountHasChanged)
            {
                int count = m_CurrentBadges.GetWeightedTotalCount();
                m_EventManager.Invoke(
                    new BadgeAmountCountChangedEvent
                    {
                        NewCount = count,
                        TotalCount = m_UserManager.CurrentUser.totalBadgeCount + count
                    }
                );
            }
        }

        private int3 EvaluateAmountBadge(WorkflowStepEndedArgs arg)
        {
            int3 buffer = int3.zero;
            if (m_PrevRepetitions == arg.executedRepetitions)
                return buffer;
            if (arg.lastStep < arg.newStep)
            {
                int amountLevel = Array.LastIndexOf<int>(m_AmountLevels, arg.executedRepetitions);
                if (amountLevel != -1)
                {
                    buffer[amountLevel] += 1;
                }
            }
            else if (arg.lastStep > arg.newStep)
            {
                int amountLevel = Array.LastIndexOf<int>(m_AmountLevels, arg.executedRepetitions);
                if (amountLevel != -1)
                {
                    buffer[amountLevel] -= 1;
                }
            }
            return buffer;
        }

        private int3 EvaluateFastBadge(WorkflowStepEndedArgs arg)
        {
            int3 buffer = int3.zero;
            if (arg.lastStep < arg.newStep)
            {
                WorkflowStepData data = m_WorkflowService.currentWorkflowData.steps[arg.lastDataStep];
                float time = m_TimeRepo.currentTime.durationsPerStep[arg.lastStep] / data.durations[1];
                if (!data.automatic && time < 0.8f)
                {
                    ++m_FastCounter;    
                }
            }
            else if (arg.lastStep > arg.newStep)
            {
                WorkflowStepData data = m_WorkflowService.currentWorkflowData.steps[arg.newDataStep];
                float time = m_TimeRepo.currentTime.durationsPerStep[arg.newStep] / data.durations[1];
                if (!data.automatic && time < 0.8f)
                {
                    --m_FastCounter;
                }

            }
            if (m_PrevRepetitions == arg.executedRepetitions)
                return buffer;
            if (arg.lastStep < arg.newStep)
            {
                int fastLevel = 0;
                while (m_FastCounter > m_FastLevels[fastLevel])
                    ++fastLevel;
                if (fastLevel != -1)
                {
                    buffer[fastLevel] += 1;
                }
                m_FastCounter = 0;
            }
            //recalculate the entire past history to properly subtract fast badge
            else if (arg.lastStep > arg.newStep)
            {
                int stepCount = arg.newDataStep - arg.lastDataStep;
                int startStep = arg.newStep - stepCount;
                m_FastCounter = 0;
                for (int i = 0; i < stepCount; ++i)
                {
                    WorkflowStepData step = m_WorkflowService.currentWorkflowData.steps[arg.lastDataStep + i];
                    float time = m_TimeRepo.currentTime.durationsPerStep[startStep + i] / step.durations[1];
                    if (!step.automatic && time < 0.8f)
                    {
                        ++m_FastCounter;
                    }
                    int fastLevel = 0;
                    while (m_FastCounter < m_FastLevels[fastLevel])
                        ++fastLevel;
                    if (fastLevel != -1)
                    {
                        buffer[fastLevel] -= 1;
                        --m_FastCounter;
                    }
                }
            }
            return buffer;
        }

        private int3 EvaluateStreakBadge(WorkflowStepEndedArgs arg)
        {
            int3 buffer = int3.zero;
            if (arg.lastStep < arg.newStep)
            {
                WorkflowStepData data = m_WorkflowService.currentWorkflowData.steps[arg.lastDataStep];
                if (data.automatic)
                    return buffer;
                ++m_StreakCounter;
                int streakLevel = Array.LastIndexOf<int>(m_StreakLevels, m_StreakCounter);
                if (streakLevel != -1)
                {
                    //remove previous streak badge
                    if (streakLevel > 0)
                        buffer[streakLevel - 1] -= 1;
                    buffer[streakLevel] += 1;
                    if (streakLevel > m_StreakLevels.Length)
                        m_StreakCounter = 0;
                }
            }
            else if (arg.lastStep > arg.newStep)
            {
                WorkflowStepData data = m_WorkflowService.currentWorkflowData.steps[arg.newDataStep];
                if (data.automatic)
                    return buffer;
                int streakLevel = Array.LastIndexOf<int>(m_StreakLevels, m_StreakCounter);
                if (streakLevel != -1)
                {
                    // readd previous streak badge
                    if (streakLevel > 0)
                        buffer[streakLevel - 1] += 1;
                    buffer[streakLevel] -= 1;
                }
                m_StreakCounter -= m_StreakCounter == 0 ? 0 : 1;
            }
            return buffer;
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler<WorkflowStepEndedArgs>(this);
            m_EventManager.AddHandler<WorkflowLoadArgs>(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler<WorkflowStepEndedArgs>(this);
            m_EventManager.RemoveHandler<WorkflowLoadArgs>(this);
        }
    }
}
