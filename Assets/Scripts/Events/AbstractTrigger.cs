// <copyright file=WorkflowTrigger.cs/>
// <copyright>
//   Copyright (c) 2019, Affective & Cognitive Institute
// 
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
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
// <date>10/31/2019 14:05</date>

using System.Collections.Generic;

namespace Aci.Unity.Events
{
    public abstract class AbstractTrigger : ITrigger
    {
        protected List<ITriggerCondition> m_Conditions = new List<ITriggerCondition>();
        /// <inheritdoc />
        public IReadOnlyList<ITriggerCondition> condition => m_Conditions;

        private List<bool> m_DesiredStates = new List<bool>();
        /// <inheritdoc />
        public IReadOnlyList<bool> desiredStates => m_DesiredStates;

        /// <inheritdoc />
        public ConditionMode mode { get; set; }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach(ITriggerCondition condition in m_Conditions)
                condition.conditionStateChanged.RemoveListener(OnConditionsChanged);
            m_Conditions.Clear();
        }

        /// <inheritdoc />
        public void AddCondition(ITriggerCondition condition, bool desiredState)
        {
            if (m_Conditions.Contains(condition))
                return;
            condition.conditionStateChanged.AddListener(OnConditionsChanged);
            m_Conditions.Add(condition);
            m_DesiredStates.Add(desiredState);
        }

        /// <inheritdoc />
        public void RemoveCondition(ITriggerCondition condition, bool desiredState)
        {
            int index = m_Conditions.IndexOf(condition);
            RemoveConditionAt(index);
        }

        /// <inheritdoc />
        public void RemoveConditionAt(int index)
        {
            if (index < 0 || index >= m_Conditions.Count)
                return;
            m_Conditions[index].conditionStateChanged.RemoveListener(OnConditionsChanged);
            m_Conditions.RemoveAt(index);
            m_DesiredStates.RemoveAt(index);
        }

        /// <inheritdoc />
        public void OnConditionsChanged()
        {
            bool result = false;
            switch (mode)
            {
                case ConditionMode.and:
                    result = EvaluateAnd();
                    break;
                case ConditionMode.or:
                    result = EvaluateOr();
                    break;
                case ConditionMode.xor:
                    result = EvaluateXor();
                    break;
            }
            if(result)
                Fire();
        }

        protected abstract void Fire();

        private bool EvaluateAnd()
        {
            for(int i = 0; i < m_Conditions.Count; ++i)
            {
                if (m_Conditions[i].state != desiredStates[i])
                    return false;
            }
            return true;
        }

        private bool EvaluateOr()
        {
            for (int i = 0; i < m_Conditions.Count; ++i)
            {
                if (m_Conditions[i].state == desiredStates[i])
                    return true;
            }
            return false;
        }

        private bool EvaluateXor()
        {
            bool result = false;
            for (int i = 0; i < m_Conditions.Count; ++i)
            {
                if (m_Conditions[i].state != desiredStates[i])
                    continue;
                if (result)
                    return false;
                result = true;
            }
            return result;
        }
    }
}
