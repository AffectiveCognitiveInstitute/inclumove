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
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Aci.Unity.Workflow.Conditions
{
    /// <summary>
    ///     Condition that triggers on a specific qc step being true.
    /// </summary>
    public class EndAssemblyCondition : ITriggerCondition, IAciEventHandler<Assembly_end_ack>, IAciEventHandler<Assembly_end_res>
    {
        private IAciEventManager m_EventManager;
        private MQTTConnector m_MqttConnector;
        private float m_Delta = 0;

        private uint m_requestId = 0;

        public EndAssemblyCondition(IAciEventManager eventManager, MQTTConnector mqttConnector)
        {
            m_EventManager = eventManager;
            m_MqttConnector = mqttConnector;

            AssemblyEndProcedure();
        }

        /// <inheritdoc />
        public UnityEvent conditionStateChanged { get; } = new UnityEvent();

        /// <inheritdoc />
        public bool state { get; private set; }

        /// <inheritdoc />
        public bool reevaluate => true;

        public void Tick()
        {
        }

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
        public void OnEvent(Assembly_end_ack arg)
        {
            if (arg.req_id != m_requestId)
                return;
            if (arg.ack == false)
            {
                // do some error handling if wanted
                return;
            }
            m_EventManager.RemoveHandler<Assembly_end_ack>(this);
            m_EventManager.AddHandler<Assembly_end_res>(this);
        }

        /// <inheritdoc />
        public void OnEvent(Assembly_end_res arg)
        {
            if (arg.req_id != m_requestId)
                return;
            if(arg.result == false)
            {
                // do some error handling if wanted
                return;
            }
            state = true;
            conditionStateChanged.Invoke();
            m_EventManager.RemoveHandler<Assembly_end_res>(this);
        }

        private void AssemblyEndProcedure()
        {
            m_requestId = m_MqttConnector.GetNewRequestId();
            Assembly_end_req request = new Assembly_end_req()
            {
                req_id = m_requestId
            };
            m_EventManager.AddHandler<Assembly_end_ack>(this);
            m_MqttConnector.SendMessage(MQTTComponents.Table, $"{MQTTTopics.Request}/assembly_end_req", JsonUtility.ToJson(request));
        }
    }
}
