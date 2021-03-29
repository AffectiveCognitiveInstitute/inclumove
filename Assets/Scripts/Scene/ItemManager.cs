// <copyright file=ItemManager.cs/>
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
using Aci.Unity.Adaptivity;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Events;
using Aci.Unity.Scene.SceneItems;
using Aci.Unity.Workflow;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Scene
{
    /// <summary>
    ///     Creates, manages and destroys instances of SceneItem in a specific scene.
    /// </summary>
    public class ItemManager : MonoBehaviour
                             , IAciEventHandler<WorkflowStepEndedArgs>
                             , IAciEventHandler<WorkflowStepFinalizedArgs>
                             , IAciEventHandler<WorkflowStopArgs>
                             , IAciEventHandler<AdaptivityLevelChangedEventArgs>
    {
        private IAciEventManager m_EventBroker;
        private ISceneItemRegistry m_ItemRegistry;
        private IWorkflowService m_WorkflowService;
        private IAdaptivityService m_AdaptivityService;

        [Zenject.Inject]
        private void Construct(IAciEventManager eventBroker
            , IWorkflowService workflowService
            , ISceneItemRegistry registry
            , IAdaptivityService adaptivityService)
        {
            m_EventBroker = eventBroker;
            m_ItemRegistry = registry;
            m_WorkflowService = workflowService;
            m_AdaptivityService = adaptivityService;

            RegisterForEvents();
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        public void RegisterForEvents()
        {
            m_EventBroker.AddHandler<WorkflowStepEndedArgs>(this);
            m_EventBroker.AddHandler<WorkflowStepFinalizedArgs>(this);
            m_EventBroker.AddHandler<WorkflowStopArgs>(this);
            m_EventBroker.AddHandler<AdaptivityLevelChangedEventArgs>(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventBroker.RemoveHandler<WorkflowStepEndedArgs>(this);
            m_EventBroker.RemoveHandler<WorkflowStepFinalizedArgs>(this);
            m_EventBroker.RemoveHandler<WorkflowStopArgs>(this);
            m_EventBroker.RemoveHandler<AdaptivityLevelChangedEventArgs>(this);
        }

        public void OnEvent(WorkflowStopArgs arg)
        {
            m_ItemRegistry.ClearRegistry();
        }

        public void OnEvent(WorkflowStepEndedArgs arg)
        {
            if(arg.lastStep != -1)
                foreach (SceneItemData data in m_WorkflowService.currentWorkflowData.steps[arg.lastDataStep].items)
                {
                    ISceneItem item = m_ItemRegistry.GetItemById(data.id);
                    item.itemTransform.gameObject.SetActive(false);
                }
        }

        public void OnEvent(WorkflowStepFinalizedArgs arg)
        {
            foreach (SceneItemData data in m_WorkflowService.currentWorkflowData.steps[arg.newDataStep].items)
            {
                ISceneItem item = m_ItemRegistry.GetItemById(data.id);
                if((item.levelable.level & (1 << m_AdaptivityService.level)) != 0)
                    item.itemTransform.gameObject.SetActive(true);
            }
        }

        public void OnEvent(AdaptivityLevelChangedEventArgs arg)
        {
            if (!m_WorkflowService.isRunning)
                return;
            foreach (SceneItemData data in m_WorkflowService.currentWorkflowData.steps[m_WorkflowService.currentDataStep].items)
            {
                ISceneItem item = m_ItemRegistry.GetItemById(data.id);
                item.itemTransform.gameObject.SetActive((item.levelable.level & (1 << m_AdaptivityService.level)) != 0);
            }
        }
    }
}