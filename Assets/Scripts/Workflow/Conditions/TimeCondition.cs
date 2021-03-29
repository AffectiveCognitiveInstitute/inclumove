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
using Aci.Unity.Util;
using UnityEngine.Events;
using Zenject;

namespace Aci.Unity.Workflow.Conditions
{
    /// <summary>
    ///     Condition that triggers after specified time in a workflowStep is met.
    /// </summary>
    public class TimeCondition : ITickable, ITriggerCondition
    {
        private ITimeProvider m_TimeProvider;
        private TickableManager m_TickableManager;

        public TimeCondition(TickableManager tickableManager, ITimeProvider timeProvider)
        {
            m_TimeProvider = timeProvider;
            m_TickableManager = tickableManager;
            m_TickableManager.Add(this);
        }

        public UnityEvent conditionStateChanged { get; } = new UnityEvent();

        /// <inheritdoc />
        public bool state { get; private set; }

        public bool reevaluate => false;

        /// <inheritdoc />
        public TimeSpan time { get; set; }

        /// <inheritdoc />
        public void Tick()
        {
            if (m_TimeProvider.elapsed < time)
                return;
            state = true;
            conditionStateChanged.Invoke();
            m_TickableManager.Remove(this);
        }

        public void Dispose()
        {
            // nothing to Dispose here
        }
    }
}
