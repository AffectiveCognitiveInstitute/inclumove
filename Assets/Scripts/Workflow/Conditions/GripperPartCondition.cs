// <copyright file=TimeCondition.cs/>
// <copyright>
//   Copyright (c) 2019, Affective & Cognitive Institute
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
// <date>10/31/2019 13:45</date>

using System;
using Aci.Unity.Events;
using Aci.Unity.Models;
using Aci.Unity.Networking;
using Aci.Unity.Util;
using BotConnector.Unity;
using Microsoft.Bot.Connector.DirectLine;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Aci.Unity.Workflow.Conditions
{
    /// <summary>
    ///     Condition for a part assembly step using a guided gripper. 
    /// </summary>
    public class GripperPartCondition : ITriggerCondition
                                      , IAciEventHandler<Assembly_step_ack>
                                      , IAciEventHandler<Assembly_step_res>
                                      , IAciEventHandler<Detect_part_ack>
                                      , IAciEventHandler<Detect_part_res>
                                      , IAciEventHandler<Guide_gripper_drop_off_req>
                                      , IAciEventHandler<Guide_gripper_pick_up_req>
                                      , IAciEventHandler<Gripper_ready_req>
                                      , IAciEventHandler<Part_picked_req>
                                      , IAciEventHandler<Assembly_part_ack>
                                      , IAciEventHandler<Assembly_part_res>
    {
        private IAciEventManager m_EventManager;
        private IBot m_Bot; 
        private MQTTConnector m_MqttConnector;

        private uint m_MouseRequestId = 0;
        private uint m_TableRequestId = 0;

        private bool m_MouseAnswerReceived = false;
        private bool m_TableAnswerReceived = false;

        public GripperPartCondition(IAciEventManager eventManager, MQTTConnector mqttConnector, IBot bot)
        {
            m_EventManager = eventManager;
            m_Bot = bot;
            m_MqttConnector = mqttConnector;

            RegisterForEvents();
        }

        /// <inheritdoc />
        public UnityEvent conditionStateChanged { get; } = new UnityEvent();

        /// <inheritdoc />
        public bool state { get; private set; }

        /// <inheritdoc />
        public bool reevaluate => false;

        /// <inheritdoc />
        public uint stepId { get; set; }

        /// <inheritdoc />
        public int partId { get; set; }

        /// <inheritdoc />
        public int gripperId { get; set; }

        /// <inheritdoc />
        public float partHeight { get; set; }

        /// <inheritdoc />
        public PCB_Position partPosition { get; set; }

        /// <inheritdoc />
        public PCB_Position pcbPosition { get; set; }

        /// <inheritdoc />
        public void Dispose()
        {
            UnregisterFromEvents();
        }

        /// <inheritdoc />
        public void RegisterForEvents()
        {
        }

        /// <inheritdoc />
        public void UnregisterFromEvents()
        {
        }

        public void OnEvent(Assembly_step_ack arg)
        {
            if (!IsRequestId(arg.req_id))
                return;
            if (arg.ack == false)
            {
                // Handle specific error and restart specific task
                return;
            }
            if (arg.req_id == m_MouseRequestId)
            {
                m_MouseAnswerReceived = true;
                m_EventManager.AddHandler<Guide_gripper_drop_off_req>(this);
                m_EventManager.AddHandler<Guide_gripper_pick_up_req>(this);
                m_EventManager.AddHandler<Gripper_ready_req>(this);
                m_EventManager.AddHandler<Part_picked_req>(this);
            }
            else if (arg.req_id == m_TableRequestId)
                m_TableAnswerReceived = true;

            if (!(m_MouseAnswerReceived && m_TableAnswerReceived))
                return;

            m_MouseAnswerReceived = false;
            m_TableAnswerReceived = false;

            m_EventManager.RemoveHandler<Assembly_step_ack>(this);
            m_EventManager.AddHandler<Assembly_step_res>(this);
        }

        public void OnEvent(Guide_gripper_drop_off_req arg)
        {
            m_Bot.SimulateMessageReceived(CreateDropOffGripperActivity(gripperId));
            Guide_gripper_drop_off_ack ack = new Guide_gripper_drop_off_ack()
            {
                req_id = arg.req_id,
                ack = true,
                error = "OK"
            };
            m_MqttConnector.SendMessage(MQTTComponents.Mouse, $"{MQTTTopics.Ack}/guide_gripper_drop_off_ack", JsonUtility.ToJson(ack));
            Guide_gripper_drop_off_res res = new Guide_gripper_drop_off_res()
            {
                req_id = arg.req_id,
                result = true,
                error = "OK"
            };
            m_MqttConnector.SendMessage(MQTTComponents.Mouse, $"{MQTTTopics.Response}/guide_gripper_drop_off_res", JsonUtility.ToJson(res));
        }

        public void OnEvent(Guide_gripper_pick_up_req arg)
        {
            m_Bot.SimulateMessageReceived(CreatePickUpGripperActivity(gripperId));
            Guide_gripper_pick_up_ack ack = new Guide_gripper_pick_up_ack()
            {
                req_id = arg.req_id,
                ack = true,
                error = "OK"
            };
            m_MqttConnector.SendMessage(MQTTComponents.Mouse, $"{MQTTTopics.Ack}/guide_gripper_pick_up_ack", JsonUtility.ToJson(ack));
            Guide_gripper_pick_up_res res = new Guide_gripper_pick_up_res()
            {
                req_id = arg.req_id,
                result = true,
                error = "OK"
            };
            m_MqttConnector.SendMessage(MQTTComponents.Mouse, $"{MQTTTopics.Response}/guide_gripper_pick_up_res", JsonUtility.ToJson(res));
        }

        public void OnEvent(Gripper_ready_req arg)
        {
            Gripper_ready_ack ack = new Gripper_ready_ack()
            {
                req_id = arg.req_id,
                ack = true,
                error = "OK"
            };
            m_MqttConnector.SendMessage(MQTTComponents.Mouse, $"{MQTTTopics.Ack}/gripper_ready_ack", JsonUtility.ToJson(ack));
            Gripper_ready_res res = new Gripper_ready_res()
            {
                req_id = arg.req_id,
                result = true,
                error = "OK"
            };
            m_MqttConnector.SendMessage(MQTTComponents.Mouse, $"{MQTTTopics.Response}/gripper_ready_res", JsonUtility.ToJson(res));
            //Send no message here
        }

        public void OnEvent(Part_picked_req arg)
        {
            Part_picked_ack ack = new Part_picked_ack()
            {
                req_id = arg.req_id,
                ack = true,
                error = "OK"
            };
            m_MqttConnector.SendMessage(MQTTComponents.Mouse, $"{MQTTTopics.Ack}/part_picked_ack", JsonUtility.ToJson(ack));
            Part_picked_res res = new Part_picked_res()
            {
                req_id = arg.req_id,
                result = true,
                error = "OK"
            };
            m_MqttConnector.SendMessage(MQTTComponents.Mouse, $"{MQTTTopics.Response}/part_picked_res", JsonUtility.ToJson(res));
            //Send no message here, we continue with assembly
        }

        public void OnEvent(Assembly_step_res arg)
        {
            if (!IsRequestId(arg.req_id))
                return;
            if (arg.result == false)
            {
                // Handle specific error and restart specific task
                return;
            }
            if (arg.req_id == m_MouseRequestId)
            {
                // remove gripper/part pick-up handlers
                m_EventManager.RemoveHandler<Guide_gripper_drop_off_req>(this);
                m_EventManager.RemoveHandler<Guide_gripper_pick_up_req>(this);
                m_EventManager.RemoveHandler<Gripper_ready_req>(this);
                m_EventManager.RemoveHandler<Part_picked_req>(this);
                // start part checking
                m_MouseRequestId = m_MqttConnector.GetNewRequestId();
                Detect_part_req partRequest = new Detect_part_req()
                {
                    req_id = m_MouseRequestId,
                    step_id = stepId,
                    part_id = partId
                };
                m_EventManager.AddHandler<Detect_part_ack>(this);
                m_MqttConnector.SendMessage(MQTTComponents.Part_cam, $"{MQTTTopics.Request}/detect_part_req", JsonUtility.ToJson(partRequest));
            }
            else if (arg.req_id == m_TableRequestId)
                m_TableAnswerReceived = true;

            // only continue once the table is finished & part cam has checked the part
            if (!(m_TableAnswerReceived & m_MouseAnswerReceived))
                return;

            StartAssembly();
        }

        public void OnEvent(Detect_part_ack arg)
        {
            if (arg.req_id != m_MouseRequestId)
                return;
            if (arg.ack == false)
            {
                // Handle specific error and restart specific task
                return;  
            }
            m_EventManager.RemoveHandler<Detect_part_ack>(this);
            m_EventManager.AddHandler<Detect_part_res>(this);
        }

        public void OnEvent(Detect_part_res arg)
        {
            if (arg.req_id != m_MouseRequestId)
                return;
            if (arg.result == false)
            {
                // Handle specific error and restart specific task
                return;
            }

            m_MouseAnswerReceived = true;
            m_EventManager.RemoveHandler<Detect_part_res>(this);

            // only continue once the table is finished & part cam has checked the part
            if (!(m_TableAnswerReceived & m_MouseAnswerReceived))
                return;

            StartAssembly();
        }

        public void OnEvent(Assembly_part_ack arg)
        {
            if (!IsRequestId(arg.req_id))
                return;
            if (arg.ack == false)
            {
                // Handle specific error and restart specific task
                return;
            }
            if (arg.req_id == m_MouseRequestId)
                m_MouseAnswerReceived = true;
            else if (arg.req_id == m_TableRequestId)
                m_TableAnswerReceived = true;

            if (!(m_TableAnswerReceived & m_TableAnswerReceived))
                return;

            m_Bot.SimulateMessageReceived(CreateStartAssemblyActivity());
            m_EventManager.RemoveHandler<Assembly_part_ack>(this);
            m_EventManager.AddHandler<Assembly_part_res>(this);
            m_MouseAnswerReceived = false;
            m_TableAnswerReceived = false;
        }

        public void OnEvent(Assembly_part_res arg)
        {
            if (!IsRequestId(arg.req_id))
                return;
            if (arg.result == false)
            {
                // Handle specific error and restart specific task
                return;
            }
            if (arg.req_id == m_MouseRequestId)
                m_MouseAnswerReceived = true;
            else if (arg.req_id == m_TableRequestId)
                m_TableAnswerReceived = true;

            if (!(m_TableAnswerReceived & m_TableAnswerReceived))
                return;

            m_EventManager.RemoveHandler<Assembly_part_res>(this);
            m_MouseAnswerReceived = false;
            m_TableAnswerReceived = false;

            state = true;
            conditionStateChanged.Invoke();
        }

        public void Initialize()
        {
            m_TableRequestId = m_MqttConnector.GetNewRequestId();
            Assembly_step_req request = new Assembly_step_req()
            {
                req_id = m_TableRequestId,
                step_id = stepId,
                part_id = partId,
                gripper_id = gripperId,
                part_height = partHeight,
                pcb_position = pcbPosition,
                part_position = partPosition,
                assembly_type = "mouse"
            };
            m_EventManager.AddHandler<Assembly_step_ack>(this);
            m_MqttConnector.SendMessage(MQTTComponents.Table, $"{MQTTTopics.Request}/assembly_step_req", JsonUtility.ToJson(request));
            m_MouseRequestId = m_MqttConnector.GetNewRequestId();
            request.req_id = m_MouseRequestId;
            m_MqttConnector.SendMessage(MQTTComponents.Mouse, $"{MQTTTopics.Request}/assembly_step_req", JsonUtility.ToJson(request));
        }


        private void StartAssembly()
        {
            m_TableAnswerReceived = false;
            m_MouseAnswerReceived = false;
            m_EventManager.RemoveHandler<Assembly_step_res>(this);
            m_EventManager.AddHandler<Assembly_part_ack>(this);
            m_TableRequestId = m_MqttConnector.GetNewRequestId();
            Assembly_part_req request = new Assembly_part_req()
            {
                req_id = m_TableRequestId,
                step_id = stepId
            };
            m_MqttConnector.SendMessage(MQTTComponents.Table, $"{MQTTTopics.Request}/assembly_part_req", JsonUtility.ToJson(request));
            m_MouseRequestId = m_MqttConnector.GetNewRequestId();
            request.req_id = m_MouseRequestId;
            m_MqttConnector.SendMessage(MQTTComponents.Mouse, $"{MQTTTopics.Request}/assembly_part_req", JsonUtility.ToJson(request));
        }

        private bool IsRequestId(uint id)
        {
            return id == m_MouseRequestId || id == m_TableRequestId;
        }

        private Activity CreateDropOffGripperActivity(int gripperId)
        {
            // Hier könnte auf Basis der Greifer Id noch eine Greiferbschreibung gewählt werden, bzw. ein Bild des Greifers
            Activity activity = new Activity();

            activity.Id = Guid.NewGuid().ToString();
            activity.Type = "message";
            activity.Text = $"Zeit den Greifer zu wechseln. Fahre die Montagehilfe zur Halterung um den alten Greifer abzulegen.";
            activity.Timestamp = DateTime.UtcNow;

            return activity;
        }

        private Activity CreatePickUpGripperActivity(int gripperId)
        {
            // Hier könnte auf Basis der Greifer Id noch eine Greiferbschreibung gewählt werden, bzw. ein Bild des Greifers
            Activity activity = new Activity();

            activity.Id = Guid.NewGuid().ToString();
            activity.Type = "message";
            activity.Text = $"Super. Fahre nun die Montagehilfe zur Halterung um den neuen Greifer aufzunehmen.";
            activity.Timestamp = DateTime.UtcNow;

            return activity;
        }

        private Activity CreateStartAssemblyActivity()
        {
            // Hier könnte auf Basis der Greifer Id noch eine Greiferbschreibung gewählt werden, bzw. ein Bild des Greifers
            Activity activity = new Activity();

            activity.Id = Guid.NewGuid().ToString();
            activity.Type = "message";
            activity.Text = $"Alles bereit. Zeit das Bauteil zu montieren.";
            activity.Timestamp = DateTime.UtcNow;

            return activity;
        }
    }
}
