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
    public class QCCondition : ITriggerCondition, IAciEventHandler<CVTriggerArgs>, ITickable
    {
        private TickableManager m_TickableManager;
        private IAciEventManager m_EventManager;
        private MQTTConnector m_MqttConnector;
        private float m_Delta = 0;
        
        public QCCondition(TickableManager tickableManager, IAciEventManager eventManager, MQTTConnector mqttConnector)
        {
            m_TickableManager = tickableManager;
            m_EventManager = eventManager;
            m_MqttConnector = mqttConnector;

            m_TickableManager.Add(this);
            RegisterForEvents();
        }

        /// <inheritdoc />
        public UnityEvent conditionStateChanged { get; } = new UnityEvent();

        /// <inheritdoc />
        public bool state { get; private set; }

        /// <inheritdoc />
        public bool reevaluate => false;

        /// <inheritdoc />
        public int triggerId { get; set; }

        public void Tick()
        {
            m_Delta += Time.deltaTime;
            if (m_Delta < 1f)
                return;
            m_Delta = 0;
            m_MqttConnector.SendQCMessage(triggerId);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            UnregisterFromEvents();
        }

        /// <inheritdoc />
        public void RegisterForEvents()
        {
            m_EventManager.AddHandler(this);
        }

        /// <inheritdoc />
        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler(this);
        }

        /// <inheritdoc />
        public void OnEvent(CVTriggerArgs arg)
        {
            if (arg.okay)
            {
                state = true;
                conditionStateChanged.Invoke();
                m_TickableManager.Remove(this);
            }
            else
            {
                state = false;
            }
        }
    }
}
