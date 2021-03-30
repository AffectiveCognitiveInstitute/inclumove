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
    ///     Condition for a part assembly step executed manually. 
    /// </summary>
    public class ManualPartCondition : ITriggerCondition
                                      , IAciEventHandler<Assembly_step_ack>
                                      , IAciEventHandler<Assembly_step_res>
                                      , IAciEventHandler<Detect_part_ack>
                                      , IAciEventHandler<Detect_part_res>
    {
        private IAciEventManager m_EventManager;
        private IBot m_Bot;
        private MQTTConnector m_MqttConnector;

        private uint m_requestId = 0;

        public ManualPartCondition(IAciEventManager eventManager, MQTTConnector mqttConnector, IBot bot)
        {
            m_EventManager = eventManager;
            m_Bot = bot;
            m_MqttConnector = mqttConnector;

            RegisterForEvents();
            PartAssembly();
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
            if (arg.req_id != m_requestId)
                return;
            if (arg.ack == false)
            {
                m_Bot.SimulateMessageReceived(CreateErrorActivity(arg.error));

                // handle error
                // switch()
                // case Gripper not detected
                // handle errors
                // get new request + send new gripper selection request
            }
            m_EventManager.RemoveHandler<Assembly_step_ack>(this);
            m_EventManager.AddHandler<Assembly_step_res>(this);
            // send chatbot message
        }

        public void OnEvent(Assembly_step_res arg)
        {
            if (arg.req_id != m_requestId)
                return;
            if(arg.result == false)
            {
                m_Bot.SimulateMessageReceived(CreateErrorActivity(arg.error));
            }
            m_EventManager.RemoveHandler<Assembly_step_res>(this);

            //wait for mousecklick
             
            m_requestId = m_MqttConnector.GetNewRequestId();
            Detect_part_req request = new Detect_part_req()
            {
                req_id = m_requestId,
                step_id = stepId,
                part_id = partId
            };
            m_EventManager.AddHandler<Detect_part_ack>(this);
            m_MqttConnector.SendMessage(MQTTComponents.Table, $"{MQTTTopics.Request}/detect_part_req", JsonUtility.ToJson(request));
        }

        public void OnEvent(Detect_part_ack arg)
        {
            if (arg.req_id != m_requestId)
                return;
            if (arg.ack == false)
            {
                m_Bot.SimulateMessageReceived(CreateErrorActivity(arg.error));
            }
            m_EventManager.RemoveHandler<Detect_part_ack>(this);
            m_EventManager.AddHandler<Detect_part_res>(this);
            // send chatbot message
            m_Bot.SimulateMessageReceived(CreateCheckPartActivity());
        }

        public void OnEvent(Detect_part_res arg)
        {
            if (arg.req_id != m_requestId)
                return;
            if (arg.result == false)
            {
                // Output error via chatbot
                m_Bot.SimulateMessageReceived(CreateErrorActivity(arg.error));
                // wait for mouseclick
                // redo Procedure
            }
            // send chatbot message
            // wait a bit
            // set state true
            state = true;
            conditionStateChanged.Invoke();
        }

        private void PartAssembly()
        {
            m_requestId = m_MqttConnector.GetNewRequestId();
            Assembly_step_req request = new Assembly_step_req()
            {
                req_id = m_requestId,
                step_id = stepId,
                part_id = partId,
                gripper_id = gripperId,
                part_height = partHeight,
                pcb_position = pcbPosition,
                part_position = partPosition,
                assembly_type = "manual"
            };
            m_EventManager.AddHandler<Assembly_step_ack>(this);
            m_MqttConnector.SendMessage(MQTTComponents.Table, $"{MQTTTopics.Request}/assembly_step_req", JsonUtility.ToJson(request));
        }

        private Activity CreateErrorActivity(string error)
        {
            Activity activity = new Activity();

            activity.Id = Guid.NewGuid().ToString();
            activity.Type = "message";
            activity.Text = $"Hoppla, Da ist wohl etwas falsch. Ich habe folgenden Fehler erhalten: {error}";
            activity.Timestamp = DateTime.UtcNow;

            return activity;
        }

        private Activity CreateCheckPartActivity()
        {
            Activity activity = new Activity();

            activity.Id = Guid.NewGuid().ToString();
            activity.Type = "message";
            activity.Text = $"Okay, ich überprüfe nun das Bauteil.";
            activity.Timestamp = DateTime.UtcNow;

            return activity;
        }
    }
}
