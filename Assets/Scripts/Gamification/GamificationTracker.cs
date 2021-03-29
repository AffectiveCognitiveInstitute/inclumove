// <copyright file=GamificationTracker.cs/>
// <copyright>
//   Copyright (c) 2018, Affective & Cognitive Institute
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software andassociated documentation files
//   (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
//   merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//   LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
//   IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// <license>MIT License</license>
// <main contributors>
//   Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>07/12/2018 05:59</date>

using System.Collections.Generic;
using System.Threading;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Events;
using Aci.Unity.Models;
using Aci.Unity.Scene;
using Aci.Unity.Util;
using Aci.Unity.Workflow;
using TMPro;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Gamification
{
    /// <summary>
    ///     Interface to user for gamification trackers.
    /// </summary>
    public abstract class GamificationTracker : MonoBehaviour
                                              , IAciEventHandler<WorkflowLoadArgs>
                                              , IAciEventHandler<WorkflowStepFinalizedArgs>
                                              , IAciEventHandler<WorkflowStartArgs>
                                              , IAciEventHandler<WorkflowStopArgs>
                                              , IAciEventHandler<DemoResetArgs>
    {
        private IAciEventManager m_Broker;

        protected ITimeProvider timeProvider;

        private IWorkflowService m_WorkflowService;

        protected int numSteps;
        protected float[] stepDurations;
        protected float totalDuration;
        protected int curStep = -1;
        protected bool detailMode = false;
        protected CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        ///     TextMesh displaying a workflow step counter
        /// </summary>
        [Tooltip("TextMesh displaying \"Current Step\" / \"Number of Steps\"")]
        public TextMeshProUGUI StepCounter;

        /// <summary>
        ///     TextMesh displaying current workflow step / total time
        /// </summary>
        [Tooltip("TextMesh displaying current elapsed time")]
        public TextMeshProUGUI Timer;

        /// <summary>
        ///     Number of steps in the current workflow.
        /// </summary>
        public int NumSteps => numSteps;

        /// <summary>
        ///     True if current tracker in detail mode.
        /// </summary>
        public bool IsDetailMode => detailMode;

        [Inject]
        private void Construct(IAciEventManager broker, ITimeProvider timeProvider, IWorkflowService workflowService)
        {
            m_Broker = broker;
            this.timeProvider = timeProvider;
            m_WorkflowService = workflowService;
        }

        /// <inhertdoc />
        public void OnEvent(WorkflowLoadArgs args)
        {
            SetSteps(m_WorkflowService.currentWorkflowData.steps);
            SetStep(0);
        }

        /// <inhertdoc />
        public void RegisterForEvents()
        {
            m_Broker?.AddHandler<WorkflowStepFinalizedArgs>(this);
            m_Broker?.AddHandler<WorkflowLoadArgs>(this);
            m_Broker?.AddHandler<WorkflowStartArgs>(this);
            m_Broker?.AddHandler<WorkflowStopArgs>(this);
            m_Broker?.AddHandler<DemoResetArgs>(this);
        }

        /// <inhertdoc />
        public void UnregisterFromEvents()
        {
            m_Broker?.RemoveHandler<WorkflowStepFinalizedArgs>(this);
            m_Broker?.RemoveHandler<WorkflowLoadArgs>(this);
            m_Broker?.RemoveHandler<WorkflowStartArgs>(this);
            m_Broker?.RemoveHandler<WorkflowStopArgs>(this);
            m_Broker?.RemoveHandler<DemoResetArgs>(this);
        }

        /// <inheritdoc />
        public void OnEvent(DemoResetArgs args)
        {
            OnEvent(new WorkflowStopArgs() {time = 0f});
            RebuildTracker();
        }

        /// <inhertdoc />
        public void OnEvent(WorkflowStartArgs args)
        {
            cts = new CancellationTokenSource();
            UpdateUIData(cts.Token);
        }

        /// <inhertdoc />
        public void OnEvent(WorkflowStepFinalizedArgs args)
        {
            SetStep(args.newStep);
        }

        /// <inhertdoc />
        public virtual void OnEvent(WorkflowStopArgs args)
        {
            cts.Cancel();
        }

        /// <summary>
        ///     Sets the current step.
        /// </summary>
        /// <param name="step">Target step.</param>
        public abstract void SetStep(int step);

        /// <summary>
        ///     Rebuilds the tracker ui.
        /// </summary>
        public abstract void RebuildTracker();

        /// <summary>
        ///     Toggles the dateiled info view.
        /// </summary>
        /// <param name="show">True if detailed mode should be shown false otherwise.</param>
        public abstract void ShowDetails(bool show);

        /// <summary>
        ///     Update method for ui. Should be async.
        /// </summary>
        /// <param name="ct">Token for stopping update.</param>
        public abstract void UpdateUIData(CancellationToken ct);

        /// <summary>
        ///     Sets the amount and duration of workflow steps.
        ///     Pauses current workflow progress.
        /// </summary>
        /// <param name="workingSteps">Target steps.</param>
        public void SetSteps(WorkflowStepData[] workingSteps)
        {
            numSteps = workingSteps.Length;
            stepDurations = new float[workingSteps.Length];
            totalDuration = 0;
            for (int i = 0; i < numSteps; ++i)
            {
                stepDurations[i] = workingSteps[i].durations[0];
                totalDuration += stepDurations[i];
            }
            
            RebuildTracker();
        }

        void OnDestroy()
        {
            UnregisterFromEvents();
        }

        /// <summary>
        ///     Deactives/Activates the
        /// </summary>
        /// <param name="activate"></param>
        public void SetActive(bool activate)
        {
            //we don't active activated objects etc.
            if (gameObject.activeSelf == activate)
                return;

            gameObject.SetActive(activate);
        }
    }
}