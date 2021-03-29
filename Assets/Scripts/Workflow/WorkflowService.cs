
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aci.Unity.Adaptivity;
using Aci.Unity.Bot;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Events;
using Aci.Unity.Gamification;
using Aci.Unity.Logging;
using Aci.Unity.Models;
using Aci.Unity.Network;
using Aci.Unity.Scene;
using Aci.Unity.Sensor;
using Aci.Unity.Util;
using Aci.Unity.Workflow.Triggers;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Workflow
{
    public class WorkflowService : IWorkflowService
                                 , IAciEventHandler<DemoResetArgs>
                                 , IAciEventHandler<AdaptivityLevelChangedEventArgs>
    {
        private IAciEventManager        m_EventBroker;
        private ITimeProvider           m_TimeProvider;
        private ITimeTrackingRepository m_TimeTrackingRepo;
        private IIdDisposalService      m_DisposalService;
        private WorkflowStepTrigger.Factory m_StepTriggerFactory;

        private CancellationTokenSource m_FinalizerTokenSource;
        private List<IStepFinalizer> m_StepFinalizers = new List<IStepFinalizer>();

        private StringBuilder m_StringBuilder = new StringBuilder();

        private int m_RepeatHandle = -1;

        private WorkflowData m_CurrentWorkflowData = WorkflowData.Empty;
        public WorkflowData currentWorkflowData => m_CurrentWorkflowData;

        private int m_LastStep = -1;
        private int m_CurrentStep = -1;
        private bool m_Finalizing = false;
        /// <inheritdoc />
        public int currentStep => m_CurrentStep;

        private int m_LastDataStep = -1;
        private int m_CurrentDataStep = -1;
        /// <inheritdoc />
        public int currentDataStep => m_CurrentDataStep;

        private int m_CurrentRepetition = 0;
        public int currentRepetition => m_CurrentRepetition;

        /// <inheritdoc />
        public float currentTime => (float)m_TimeProvider.elapsedTotal.TotalMinutes;

        private bool m_IsRunning = false;

        /// <inheritdoc />
        public bool isRunning => m_IsRunning;

        [Inject]
        public void Construct(IAciEventManager broker
                            , ITimeProvider timeProvider
                            , ITimeTrackingRepository timeTrackingRepo
                            , IIdDisposalService disposalService
                            , WorkflowStepTrigger.Factory triggerFactory)
        {
            m_EventBroker = broker;
            m_TimeProvider = timeProvider;
            m_TimeTrackingRepo = timeTrackingRepo;
            m_DisposalService = disposalService;
            m_StepTriggerFactory = triggerFactory;
            RegisterForEvents();
        }

        /// <inheritdoc />
        public void RegisterForEvents()
        {
            m_EventBroker.AddHandler<DemoResetArgs>(this);
            m_EventBroker.AddHandler<AdaptivityLevelChangedEventArgs>(this);
        }

        /// <inheritdoc />
        public void UnregisterFromEvents()
        {
            m_EventBroker.RemoveHandler<DemoResetArgs>(this);
            m_EventBroker.RemoveHandler<AdaptivityLevelChangedEventArgs>(this);
        }

        /// <inheritdoc />
        public void OnEvent(EmotionChanged arg)
        {
            //TODO: If over time, notify bot
        }

        /// <inheritdoc />
        public void OnEvent(DemoResetArgs arg)
        {
            if (!m_IsRunning)
                return;
            StopWork();
            SetWorkflowData(WorkflowData.Empty);
        }

        /// <inheritdoc />
        public void SetWorkflowData(WorkflowData data)
        {
            if(m_IsRunning)
                StopWork();

            m_CurrentWorkflowData = data;

            m_CurrentStep = m_CurrentDataStep = m_CurrentRepetition = m_RepeatHandle = 0;
            m_TimeTrackingRepo.currentTime = new TimeTrackingData(m_CurrentWorkflowData.numTotalSteps);
            m_TimeProvider.Reset(true);
            m_TimeProvider.paused = true;

            // publish workflow loaded event
            m_EventBroker?.Invoke(new WorkflowLoadArgs()
            {
                msg = m_CurrentWorkflowData.name
            });
        }

        /// <inheritdoc />
        public void StartWork()
        {
            // don't bother if started or no workflow loaded
            if (m_IsRunning || m_CurrentWorkflowData == WorkflowData.Empty)
                return;

            m_FinalizerTokenSource = new CancellationTokenSource();
            m_TimeProvider.paused = false;
            m_IsRunning = true;
            m_EventBroker?.Invoke(new WorkflowStartArgs());
            m_EventBroker?.Invoke(new WorkflowStepFinalizedArgs()
            {
                lastStep = -1,
                lastDataStep = -1,
                newStep = m_CurrentStep,
                newDataStep = m_CurrentDataStep
            });
            m_StepTriggerFactory.Create(m_CurrentWorkflowData.steps[currentDataStep]);
            AciLog.Log("WorkflowManager", "Started Workflow");
        }

        /// <inheritdoc />
        public void StopWork()
        {
            m_IsRunning = false;
            m_TimeProvider.paused = true;
            m_FinalizerTokenSource?.Cancel();
            m_EventBroker?.Invoke(new WorkflowStopArgs() { time = 0f });
        }

        /// <inheritdoc />
        public async void SetStep(int step)
        {
            // don't set step if finalizing
            if (m_Finalizing)
                return;

            // buffer the previous step numbers
            int m_LastStep = m_CurrentStep;
            int m_LastDataStep = m_CurrentDataStep;

            // pause the time until we are done switching steps
            bool pauseState = m_TimeProvider.paused;
            m_TimeProvider.paused = true;

            // now advance/move back to the new step
            Action functor = null;
            int diff = 0;


            if (step > m_LastStep)
            {
                functor = AdvanceStep;
                diff = step - m_LastStep;
            }
            else if (step < m_LastStep)
            {
                functor = RetreatStep;
                diff = m_LastStep - step;
            }
            else
            {
                //restore state before attempt
                m_TimeProvider.paused = pauseState;
                return;
            }
            
            for (int i = 0; i < diff; ++i)
            {
                functor.Invoke();
            }

            // record time for previous step
            float stepTime = (float)m_TimeProvider.elapsed.TotalSeconds;
            m_TimeTrackingRepo.currentTime.durationsPerStep[m_LastStep] = stepTime;

            m_Finalizing = true;

            // invoke step event
            m_EventBroker?.Invoke(new WorkflowStepEndedArgs()
            {
                lastStep = m_LastStep
              , lastDataStep = m_LastDataStep
              , newStep = m_CurrentStep
              , newDataStep = m_CurrentDataStep
              , executedRepetitions = m_CurrentRepetition
            });

            try
            {
                await WaitForFinalizers(m_FinalizerTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                // Cancellation means we ended the workflow(quit), don't finalize
                m_Finalizing = false;
                return;
            }

            m_Finalizing = false;

            // stop workflow if we reached the end
            if (m_CurrentStep >= currentWorkflowData.numTotalSteps)
            {
                if (m_LastDataStep != -1)
                    m_DisposalService.Dispose(m_CurrentWorkflowData.steps[m_LastDataStep].id);
                StopWork();
                return;
            }

            // restore time for current step
            m_TimeProvider.Reset(false);
            float currentTime = m_TimeTrackingRepo.currentTime.durationsPerStep[m_CurrentStep];
            m_TimeProvider.paused = false;
            m_TimeProvider.Add(TimeSpan.FromSeconds(currentTime));

            LogStepTimings();
            AciLog.Log("WorkflowManager", $"Workflow Step, {m_CurrentStep}");
            AciLog.Log("WorkflowManager", $"Data Step: {m_CurrentDataStep}");

            if(m_LastDataStep != -1)
                m_DisposalService.Dispose(m_CurrentWorkflowData.steps[m_LastDataStep].id);

            m_StepTriggerFactory.Create(m_CurrentWorkflowData.steps[currentDataStep]);

            // invoke step event
            m_EventBroker?.Invoke(new WorkflowStepFinalizedArgs()
            {
                lastStep = m_LastStep
              , lastDataStep = m_LastDataStep
              , newStep = m_CurrentStep
              , newDataStep = m_CurrentDataStep
              , executedRepetitions = m_CurrentRepetition
            });
        }

        public async Task WaitForFinalizers(CancellationToken token)
        {
            bool result;
            do
            {
                if (token.IsCancellationRequested)
                    throw new TaskCanceledException();
                await new WaitForEndOfFrame();
                result = true;
                foreach (IStepFinalizer finalizer in m_StepFinalizers)
                    result &= finalizer.finalized;
            } while (!result);
        }

        private void AdvanceStep()
        {
            int maxRepetitions = m_CurrentWorkflowData.steps[m_CurrentDataStep].repetitions;
            // decrement repetitions of current data step
            int currentRepetitions = --m_CurrentWorkflowData.steps[m_CurrentDataStep].currentRepetitions;
            // increment the total steps so far
            ++m_CurrentStep;
            // increment the data step
            ++m_CurrentDataStep;
            // if we have spent all repetitions for step step update the repeat handle
            if (currentRepetitions == -1)
            {
                m_RepeatHandle = m_CurrentDataStep;
                // if we had more than 0 repetitions previously and now don't, we used up all repetitions, set max
                if ( maxRepetitions > 0 && (m_CurrentDataStep == m_CurrentWorkflowData.steps.Length || m_CurrentWorkflowData.steps[m_CurrentDataStep].repetitions <= 0))
                {
                    m_CurrentRepetition = maxRepetitions + 1;
                }
            }
            // else if the next step has different repetitions from our handle go to repeat handle
            else if (m_CurrentDataStep == m_CurrentWorkflowData.steps.Length ||
                     m_CurrentWorkflowData.steps[m_CurrentDataStep].repetitions !=
                     m_CurrentWorkflowData.steps[m_CurrentDataStep - 1].repetitions)
            {
                m_CurrentDataStep = m_RepeatHandle;
                m_CurrentRepetition = maxRepetitions - currentRepetitions;
            }
        }

        private void RetreatStep()
        {
            int maxRepetitions = m_CurrentWorkflowData.steps[m_CurrentDataStep].repetitions;
            // decrement the steps
            --m_CurrentStep;
            --m_CurrentDataStep;

            // if we go past the repeat handle and 
            if (m_CurrentDataStep+1 == m_RepeatHandle)
            {
                // if our repetitions are not full yet
                if (m_CurrentWorkflowData.steps[m_CurrentDataStep+1].currentRepetitions <
                    m_CurrentWorkflowData.steps[m_CurrentDataStep+1].repetitions)
                {
                    ++m_CurrentDataStep;
                    while (m_CurrentDataStep < m_CurrentWorkflowData.steps.Length && m_CurrentWorkflowData.steps[m_CurrentDataStep].repetitions == m_CurrentWorkflowData.steps[m_CurrentDataStep + 1].repetitions)
                        ++m_CurrentDataStep;
                    --m_CurrentRepetition;
                }
                else
                {
                    m_RepeatHandle = m_CurrentDataStep;
                }
            }
            // increment the repetitions
            ++m_CurrentWorkflowData.steps[m_CurrentDataStep].currentRepetitions;
        }

        private void LogStepTimings()
        {
            m_StringBuilder.Append("Step Timings,");
            for (int i = 0; i < m_TimeTrackingRepo.currentTime.durationsPerStep.Length; ++i)
            {
                m_StringBuilder.Append("Step ").Append(i).Append(",").Append(m_TimeTrackingRepo
                                                                             .currentTime.durationsPerStep[i]
                                                                             .ToString(new System.Globalization.CultureInfo("en-US"))).Append(",");
            }
            m_StringBuilder.Remove(m_StringBuilder.Length - 1, 1);
            AciLog.Log("WorkflowManager", m_StringBuilder.ToString());
            m_StringBuilder.Clear();
        }

        public void RegisterFinalizer(IStepFinalizer finalizer)
        {
            m_StepFinalizers.Add(finalizer);
        }

        public void UnregisterFinalizer(IStepFinalizer finalizer)
        {
            m_StepFinalizers.Remove(finalizer);
        }

        public void OnEvent(AdaptivityLevelChangedEventArgs arg)
        {
            m_DisposalService.Dispose(m_CurrentWorkflowData.steps[currentDataStep].id);
            m_StepTriggerFactory.Create(m_CurrentWorkflowData.steps[currentDataStep]);
        }
    }
}
