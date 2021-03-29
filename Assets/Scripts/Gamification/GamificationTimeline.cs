// <copyright file=GamificationTimeline.cs/>
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

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Events;
using Aci.Unity.UserInterface.Animation;
using Aci.Unity.Util;
using Aci.Unity.Workflow;
using Aci.Unity.Adaptivity;

namespace Aci.Unity.Gamification
{
    public class GamificationTimeline : MonoBehaviour
                                      , ITimeline
                                      , IQueueableAnimation
                                      , IAciEventHandler<WorkflowLoadArgs>
                                      , IAciEventHandler<WorkflowStepEndedArgs>
                                      , IAciEventHandler<WorkflowStepFinalizedArgs>
                                      , IAciEventHandler<AdaptivityLevelChangedEventArgs>
    {
        public class Factory : PlaceholderFactory<WorkflowStepData, ITimelineItemViewController> { }

        private Factory m_TimelineItemFactory;
        private ITimelineItemViewController[] m_TimelineItems = new ITimelineItemViewController[0];
        private IAciEventManager m_EventManager;
        private IWorkflowService m_WorkflowService;
        private IAdaptivityService m_AdaptivityService;
        private ITimeProvider m_TimeProvider;
        private IAnimationQueue m_AnimationQueue = new AnimationQueue();

        private int m_PrevRepetitions = 0;
        private bool m_AnimateBubbleUp = false;

        [SerializeField]
        private GameObject m_TimelineBackgroundPrefab;
        
        [SerializeField]
        private Transform m_TimelineBackgroundContainer;
        
        [SerializeField]
        private Transform m_TimelineItemContainer;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventManager
                             , IWorkflowService workflowService
                             , IAdaptivityService adaptivityService
                             , ITimeProvider timeProvider
                             , Factory timelineFactory)
        {
            m_EventManager = eventManager;
            m_WorkflowService = workflowService;
            m_AdaptivityService = adaptivityService;
            m_TimeProvider = timeProvider;
            m_TimelineItemFactory = timelineFactory;
            RegisterForEvents();
        }
        
        /// <inheritdoc />
        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        /// <inheritdoc />
        public void SetData(WorkflowStepData[] data)
        {
            m_AnimationQueue.Clear();
            m_PrevRepetitions = 0;

            for (int i = 0; i < m_TimelineItems.Length; ++i)
            {
                Destroy(m_TimelineItems[i].gameObject);
            }

            while (m_TimelineBackgroundContainer.childCount > 0)
            {
                Destroy(m_TimelineBackgroundContainer.GetChild(0));
            }

            Array.Resize(ref m_TimelineItems, data?.Length ?? 0);
            QueueableAnimationSynchronizer animSync = QueueableAnimationSynchronizer.Pool.Spawn();
            for(int i = 0; i < m_TimelineItems.Length; ++i)
            {
                m_TimelineItems[i] = m_TimelineItemFactory.Create(data[i]);
                m_TimelineItems[i].gameObject.transform.SetParent(m_TimelineItemContainer, false);
                if (data[i].repetitions > 0)
                {
                    animSync.Append(m_TimelineItems[i].progressResetAnimation);
                }
            }
            m_AnimationQueue.Enqueue(animSync);
            CalculateSpacing(data);
        }

        private void CalculateSpacing(WorkflowStepData[] data)
        {
            if (data == null)
                return;

            Rect dimensions = (transform as RectTransform).rect;
            float totalDuration = 0;
            int automaticSteps = 0;
            foreach (WorkflowStepData stepData in data)
            {
                totalDuration += stepData.automatic ? 0 : stepData.durations[m_AdaptivityService.level];
                if (stepData.automatic)
                    ++automaticSteps;
            }

            float elementHeight = dimensions.height;
            float blockedSpace = 0;
            do
            {
                blockedSpace = data.Length * elementHeight + elementHeight * 0.05f * (data.Length - 1);
                elementHeight *= 0.75f;
            } while (blockedSpace > dimensions.width);

            elementHeight /= 0.75f;

            Vector2 normalizingFactor = new Vector2 (1f / dimensions.width, 1f / dimensions.height); 
            float durationFactor = ((dimensions.width - blockedSpace) / totalDuration) * normalizingFactor.x;

            float normalizedHeight = elementHeight * normalizingFactor.y;
            float stepBaseSize = elementHeight * normalizingFactor.x;
            float spacerSize = stepBaseSize * 0.05f;
            Vector2 position = new Vector2(0f, (1f - normalizedHeight) * 0.5f);
            
            int prevRep = 0;
            Vector2 bgPos = Vector2.zero;
            for (int i = 0; i < data.Length; ++i)
            {
                RectTransform trans = m_TimelineItems[i].gameObject.transform as RectTransform;
                m_TimelineItems[i].position = position;
                m_TimelineItems[i].progress = 0;
                m_TimelineItems[i].completed = false;
                m_TimelineItems[i].size = new Vector2((data[i].automatic ? stepBaseSize : stepBaseSize + data[i].durations[m_AdaptivityService.level] * durationFactor), normalizedHeight);
                m_TimelineItems[i].hasParticles = data[i].repetitions > 0;
                if(prevRep != data[i].repetitions)
                {
                    if(bgPos == Vector2.zero)
                        bgPos = position;
                    else
                    {
                        // create timeline repetition background
                        GameObject go = GameObject.Instantiate(m_TimelineBackgroundPrefab);
                        RectTransform bgTrans = go.transform as RectTransform;
                        bgTrans.SetParent(m_TimelineBackgroundContainer);
                        bgTrans.anchorMin = bgPos;
                        bgTrans.anchorMax = position + Vector2.up * m_TimelineItems[i].size.y;
                        bgTrans.offsetMin = Vector2.zero;
                        bgTrans.offsetMax = Vector2.zero;
                        bgTrans.localScale = Vector3.one;
                        // reset repetitions
                        prevRep = 0;
                    }
                }
                prevRep = data[i].repetitions;
                position.x += m_TimelineItems[i].size.x + spacerSize;
            }
        }

        /// <inheritdoc />
        public void RegisterForEvents()
        {
            m_EventManager.AddHandler<WorkflowLoadArgs>(this);
            m_EventManager.AddHandler<WorkflowStepEndedArgs>(this);
            m_EventManager.AddHandler<WorkflowStepFinalizedArgs>(this);
            m_EventManager.AddHandler<AdaptivityLevelChangedEventArgs>(this);
        }

        /// <inheritdoc />
        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler<WorkflowLoadArgs>(this);
            m_EventManager.RemoveHandler<WorkflowStepEndedArgs>(this);
            m_EventManager.RemoveHandler<WorkflowStepFinalizedArgs>(this);
            m_EventManager.RemoveHandler<AdaptivityLevelChangedEventArgs>(this);
        }

        /// <inheritdoc />
        public void OnEvent(WorkflowLoadArgs arg)
        {
            SetData(m_WorkflowService.currentWorkflowData.steps);
        }

        /// <inheritdoc />
        public void OnEvent(WorkflowStepEndedArgs arg)
        {
            if (arg.lastDataStep == -1)
                return;
            m_TimelineItems[arg.lastDataStep].active = false;
            if (arg.lastStep > arg.newStep)
                return;
            m_TimelineItems[arg.lastDataStep].completed = true;
            if(m_PrevRepetitions < arg.executedRepetitions)
            {
                m_AnimateBubbleUp = true;
                m_PrevRepetitions = arg.executedRepetitions;
            }
        }

        /// <inheritdoc />
        public void OnEvent(WorkflowStepFinalizedArgs arg)
        {
            if (arg.lastDataStep != -1)
            {
                if (!(arg.lastDataStep < arg.newDataStep))
                {
                    for (int i = arg.newDataStep; i <= arg.lastDataStep; ++i)
                    {
                        m_TimelineItems[i].Reset();
                    }
                }
            }
            m_TimelineItems[arg.newDataStep].active = true;
        }

        /// <inheritdoc />
        public void Update()
        {
            if (!m_WorkflowService.isRunning || m_TimeProvider.paused)
                return;
            m_TimelineItems[m_WorkflowService.currentDataStep].progress =
                (float)m_TimeProvider.elapsed.TotalSeconds / m_WorkflowService
                                                 .currentWorkflowData.steps[m_WorkflowService.currentDataStep]
                                                 .durations[m_AdaptivityService.level];
        }

        /// <inheritdoc />
        public async Task Play()
        {
            if (!m_AnimateBubbleUp)
                return;
            await m_AnimationQueue.PlaySafe();
            m_AnimateBubbleUp = false;
        }

        public IQueueableAnimation GetItemProgressAnimation(int dataStep)
        {
            return m_TimelineItems[dataStep].progressCompleteAnimation;
        }

        public void OnEvent(AdaptivityLevelChangedEventArgs arg)
        {
            if (!m_WorkflowService.isRunning)
                return;
            CalculateSpacing(m_WorkflowService.currentWorkflowData.steps);
        }
    }
}