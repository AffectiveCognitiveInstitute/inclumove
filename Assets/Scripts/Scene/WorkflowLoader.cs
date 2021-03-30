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

using System.IO;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Events;
using Aci.Unity.Logging;
using Aci.Unity.Models;
using Aci.Unity.Networking;
using Aci.Unity.Scene.SceneItems;
using Aci.Unity.Util;
using Aci.Unity.Workflow;
using UnityEngine;
using WebSocketSharp;
using Zenject;
using SceneItem = Aci.Unity.Scene.SceneItems.SceneItem;

namespace Aci.Unity.Scene
{
    public class WorkflowLoader : MonoBehaviour,
                                  IAciEventHandler<Reset_ack>,
                                  IAciEventHandler<Reset_res>,
                                  IAciEventHandler<Assembly_order_ack>,
                                  IAciEventHandler<Assembly_order_res>
    {
        private SceneItem.Factory m_SceneItemFactory;

        private IAciEventManager m_EventManager;

        private IWorkflowService m_WorkflowService;

        private MQTTConnector m_MqttConnector;

        private IConfigProvider m_ConfigProvider;

        private ISceneItemRegistry m_ItemRegistry;

        private uint m_requestId;

        [ConfigValue("workflowDirectory")]
        public string workflowDirectory { get; set; } = "";

        [Inject]
        private void Construct(IConfigProvider    configProvider, MQTTConnector mqttConnector,
                               IWorkflowService   workflowService, SceneItem.Factory sceneItemFactory,
                               ISceneItemRegistry itemRegistry, IAciEventManager manager)
        {
            m_ConfigProvider = configProvider;
            m_WorkflowService = workflowService;
            m_SceneItemFactory = sceneItemFactory;
            m_ItemRegistry = itemRegistry;
            m_MqttConnector = mqttConnector;
            m_EventManager = manager;

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

            m_requestId = m_MqttConnector.GetNewRequestId();
            Reset_req request = new Reset_req()
            {
                req_id = m_requestId
            };
            m_EventManager.AddHandler<Reset_ack>(this);
            m_MqttConnector.SendMessage(MQTTComponents.Table, $"{MQTTTopics.Request}/reset_req", JsonUtility.ToJson(request));
        }

        public void RegisterForEvents()
        {
        }

        public void UnregisterFromEvents()
        {
        }

        public void OnEvent(Assembly_order_ack arg)
        {
            if (arg.req_id != m_requestId)
                return;
            if (arg.ack == false)
            {
                AciLog.LogError("Assembly_order_ack", arg.error);
                return;
            }
            m_EventManager.RemoveHandler<Assembly_order_ack>(this);
            m_EventManager.AddHandler<Assembly_order_res>(this);
            
        }

        public void OnEvent(Assembly_order_res arg)
        {
            if (arg.req_id != m_requestId)
                return;
            if (arg.result == false)
            {
                AciLog.LogError("Assembly_order_res", arg.error);
                return;
            }
            m_EventManager.RemoveHandler<Assembly_order_res>(this);
            m_WorkflowService.StartWork();
        }

        public void OnEvent(Reset_ack arg)
        {
            if (arg.req_id != m_requestId)
                return;
            if (arg.ack == false) { 
                AciLog.LogError("Reset_ack", arg.error);
                return;
            }
            m_EventManager.RemoveHandler<Reset_ack>(this);
            m_EventManager.AddHandler<Reset_res>(this);
        }

        public void OnEvent(Reset_res arg)
        {
            if (arg.req_id != m_requestId)
                return;
            if (arg.result == false)
            {
                AciLog.LogError("Reset_res_res", arg.error);
                return;
            }
            m_EventManager.RemoveHandler<Reset_res>(this);

            m_requestId = m_MqttConnector.GetNewRequestId();
            Assembly_order_req request = new Assembly_order_req()
            {
                req_id = m_requestId,
                order_id = m_WorkflowService.currentWorkflowData.orderId,
                reference_marks = m_WorkflowService.currentWorkflowData.referenceMarks
            };
            m_EventManager.AddHandler<Assembly_order_ack>(this);
            m_MqttConnector.SendMessage(MQTTComponents.Table, $"{MQTTTopics.Request}/assembly_order_req", JsonUtility.ToJson(request));
        }
    }
}