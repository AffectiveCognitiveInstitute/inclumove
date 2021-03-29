// <copyright file=WorkflowTitleViewController.cs/>
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
//   James Gay, Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>08/14/2018 11:50</date>

using Aci.Unity.Events;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Models;
using Aci.Unity.Scene;
using Aci.Unity.UI.Localization;
using Aci.Unity.Workflow;
using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    /// <summary>
    ///     Class responsible for updating the title of the current workflow
    /// </summary>
    public class WorkflowTitleViewController : MonoBehaviour
                                             , IAciEventHandler<WorkflowLoadArgs>
                                             , IAciEventHandler<WorkflowStopArgs>
                                             , IAciEventHandler<WorkflowStepEndedArgs>
                                             , IAciEventHandler<LocalizationChangedArgs>
                                             , IAciEventHandler<DemoResetArgs>
    {
        public string defaultTitle;

        private IAciEventManager     m_EventBroker;
        private ILocalizationManager m_LocalizationManager;

        [SerializeField]
        private TextMeshProUGUI m_Text;

        private bool            m_WorkflowCompleted;
        private IWorkflowService m_WorkflowService;

        [Inject]
        private void Construct(IAciEventManager     eventBroker,
                               IWorkflowService     workflowService,
                               ILocalizationManager localizationManager)
        {
            m_EventBroker = eventBroker;
            m_WorkflowService = workflowService;
            m_LocalizationManager = localizationManager;
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
            m_EventBroker?.AddHandler<WorkflowLoadArgs>(this);
            m_EventBroker?.AddHandler<WorkflowStepEndedArgs>(this);
            m_EventBroker?.AddHandler<WorkflowStopArgs>(this);
            m_EventBroker?.AddHandler<LocalizationChangedArgs>(this);
            m_EventBroker?.AddHandler<DemoResetArgs>(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventBroker?.RemoveHandler<WorkflowLoadArgs>(this);
            m_EventBroker?.RemoveHandler<WorkflowStepEndedArgs>(this);
            m_EventBroker?.RemoveHandler<WorkflowStopArgs>(this);
            m_EventBroker?.RemoveHandler<LocalizationChangedArgs>(this);
            m_EventBroker?.RemoveHandler<DemoResetArgs>(this);
        }

        public void OnEvent(WorkflowLoadArgs args)
        {
            WorkflowData wf = m_WorkflowService.currentWorkflowData;
            m_WorkflowCompleted = false;
            if (wf == null)
                return;

            // Update title
            UpdateTitle(wf.name, m_WorkflowService.currentStep + 1, wf.numTotalSteps);
        }

        public void OnEvent(WorkflowStepEndedArgs args)
        {
            //update title
            WorkflowData wf = m_WorkflowService.currentWorkflowData;
            UpdateTitle(wf.name, m_WorkflowService.currentStep + 1, wf.numTotalSteps);
            //check for completion
            OnWorkflowCompleted();
        }

        public void OnEvent(WorkflowStopArgs args)
        {
            OnWorkflowCompleted();
        }

        public void OnEvent(LocalizationChangedArgs args)
        {
            if (m_WorkflowCompleted)
                OnWorkflowCompleted();
        }

        public void OnEvent(DemoResetArgs args)
        {
            m_Text.text = defaultTitle;
        }

        private void UpdateTitle(string name, int step, int totalSteps)
        {
            m_Text.text = $"{name} {step}/{totalSteps}";
        }

        private void OnWorkflowCompleted()
        {
            WorkflowData wf = m_WorkflowService.currentWorkflowData;
            try
            {
                if (m_WorkflowService.currentStep == wf.numTotalSteps)
                {
                    m_WorkflowCompleted = true;
                    m_Text.text = m_LocalizationManager.GetLocalized("{{workflow_completed}}");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}