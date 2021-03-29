// <copyright file=WorkflowLoader.cs/>
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
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Network;
using Aci.Unity.Scene.SceneItems;
using Aci.Unity.Util;
using Aci.Unity.Workflow;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using Zenject;

namespace Aci.Unity.Scene
{
    public class WorkflowLoader : MonoBehaviour
    {
        private SceneItem.Factory m_SceneItemFactory;

        private IWorkflowService m_WorkflowService;

        private INetworkPublisher m_INetworkPublisher;

        private IConfigProvider m_ConfigProvider;

        private ISceneItemRegistry m_ItemRegistry;

        [ConfigValue("workflowDirectory")]
        public string workflowDirectory { get; set; } = "";

        [Inject]
        private void Construct(IConfigProvider    configProvider,  INetworkPublisher    publisher,
                               IWorkflowService   workflowService, SceneItem.Factory sceneItemFactory,
                               ISceneItemRegistry itemRegistry)
        {
            m_ConfigProvider = configProvider;
            m_INetworkPublisher = publisher;
            m_WorkflowService = workflowService;
            m_SceneItemFactory = sceneItemFactory;
            m_ItemRegistry = itemRegistry;

            m_ConfigProvider?.RegisterClient(this);
            //write default values to config if no config values were loaded
            if (workflowDirectory.IsNullOrEmpty())
                m_ConfigProvider?.ClientDirty(this);
        }

        public void LoadWorkflow(string fileName = null)
        {
            string filePath = workflowDirectory + "/" + fileName;
            if (!File.Exists(filePath))
                return;
            m_ItemRegistry.ClearRegistry();

            string stringData = File.ReadAllText(filePath);

            WorkflowData data = JsonUtility.FromJson<WorkflowData>(stringData);

            foreach (WorkflowStepData stepData in data.steps)
            {
                foreach (SceneItemData itemData in stepData.items)
                {
                    ISceneItem item = m_SceneItemFactory.Create(itemData);
                    item.itemTransform.gameObject.SetActive(false);
                }
            }

            m_WorkflowService.SetWorkflowData(data);
            m_WorkflowService.StartWork();
        }
    }
}