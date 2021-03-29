// <copyright file=WorkflowStepTriggerFactory.cs/>
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
// <date>10/31/2019 17:07</date>

using System;
using System.Collections.Generic;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Events;
using Aci.Unity.Util;
using Aci.Unity.Workflow.Conditions;
using Zenject;

namespace Aci.Unity.Workflow.Triggers
{
    public class WorkflowStepTriggerFactory : IFactory<WorkflowStepData, List<ITrigger>>
    {
        private IInstantiator m_Instantiator;
        private IIdDisposalService m_DisposalService;

        [Zenject.Inject]
        private void Construct(IInstantiator instantiator, IIdDisposalService disposalService)
        {
            m_Instantiator = instantiator;
            m_DisposalService = disposalService;
        }

        public List<ITrigger> Create(WorkflowStepData param)
        {
            List<ITrigger> triggerList = new List<ITrigger>();
            if (param.automatic)
            {
                triggerList.Add(CreateAutoTrigger(param));
            }else if(param.triggerId != -1)
            {
                triggerList.Add(CreateCVTrigger(param));
            }

            return triggerList;
        }

        private ITrigger CreateAutoTrigger(WorkflowStepData param)
        {
            WorkflowStepTrigger trigger = m_Instantiator.Instantiate<WorkflowStepTrigger>();
            TimeCondition condition = m_Instantiator.Instantiate<TimeCondition>();
            condition.time = TimeSpan.FromSeconds(param.durations[0]);
            trigger.AddCondition(condition, true);
            trigger.advance = true;
            trigger.mode = ConditionMode.and;
            m_DisposalService.Register(trigger, param.id);
            m_DisposalService.Register(condition, param.id);
            return trigger;
        }

        private ITrigger CreateCVTrigger(WorkflowStepData param)
        {
            WorkflowStepTrigger trigger = m_Instantiator.Instantiate<WorkflowStepTrigger>();
            QCCondition condition = m_Instantiator.Instantiate<QCCondition>();
            condition.triggerId = param.triggerId;
            trigger.AddCondition(condition, true);
            trigger.advance = true;
            trigger.mode = ConditionMode.and;
            m_DisposalService.Register(trigger, param.id);
            m_DisposalService.Register(condition, param.id);
            return trigger;
        }
    }
}