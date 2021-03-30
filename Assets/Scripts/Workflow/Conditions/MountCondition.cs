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
    ///     Condition that triggers on a specific qc step being true.
    /// </summary>
    public class MountCondition : ITriggerCondition, IAciEventHandler<Insert_board_ack>, IAciEventHandler<Insert_board_res>, IAciEventHandler<Status>
    {
        private IAciEventManager m_EventManager;
        private IBot m_Bot;
        private MQTTConnector m_MqttConnector;

        private uint m_requestId = 0;

        public MountCondition(IAciEventManager eventManager, MQTTConnector mqttConnector, IBot bot)
        {
            m_EventManager = eventManager;
            m_Bot = bot;
            m_MqttConnector = mqttConnector;

            MountProcedure();
        }

        /// <inheritdoc />
        public UnityEvent conditionStateChanged { get; } = new UnityEvent();

        /// <inheritdoc />
        public bool state { get; private set; }

        /// <inheritdoc />
        public bool reevaluate => true;


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

        /// <inheritdoc />
        public void OnEvent(Insert_board_ack arg)
        {
            if (arg.req_id != m_requestId || arg.ack == false)
                return;
            m_EventManager.RemoveHandler<Insert_board_ack>(this);
            m_EventManager.AddHandler<Status>(this);
            // send chatbot message
        }

        /// <inheritdoc />
        public void OnEvent(Status arg)
        {
            if (arg.state != 2 || arg.error != "Waiting for operator to insert board")
                return;

            // send chatbot message
            Activity activity = new Activity();

            activity.Id = Guid.NewGuid().ToString();
            activity.Type = "message";
            activity.Text = $"Lege nun die Platine in die Halterung ein und schliesse danach die Abdeckung.";
            activity.Timestamp = DateTime.UtcNow;

            m_Bot.SimulateMessageReceived(activity);

            m_EventManager.RemoveHandler<Status>(this);
            m_EventManager.AddHandler<Insert_board_res>(this);
        }

        /// <inheritdoc />
        public void OnEvent(Insert_board_res arg)
        {
            if (arg.req_id != m_requestId || arg.result == false)
                return;
            // send chatbot message
            // wait a bit
            // set state true
            state = true;
            conditionStateChanged.Invoke();
            m_EventManager.RemoveHandler<Insert_board_res>(this);
        }

        private void MountProcedure()
        {
            m_requestId = m_MqttConnector.GetNewRequestId();
            // send insert board req
            Insert_board_req request = new Insert_board_req()
            {
                req_id = m_requestId
            };
            m_EventManager.AddHandler<Insert_board_ack>(this);
            m_MqttConnector.SendMessage(MQTTComponents.Table, $"{MQTTTopics.Request}/insert_board_req", JsonUtility.ToJson(request));
        }
    }
}
