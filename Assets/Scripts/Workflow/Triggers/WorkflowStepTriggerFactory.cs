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
using System.Linq;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Events;
using Aci.Unity.Models;
using Aci.Unity.Util;
using Aci.Unity.Workflow.Conditions;
using Zenject;

namespace Aci.Unity.Workflow.Triggers
{
    public class WorkflowStepTriggerFactory : IFactory<WorkflowStepData, WorkflowPartData[], List<ITrigger>>
    {
        private IInstantiator m_Instantiator;
        private IIdDisposalService m_DisposalService;

        [Zenject.Inject]
        private void Construct(IInstantiator instantiator, IIdDisposalService disposalService)
        {
            m_Instantiator = instantiator;
            m_DisposalService = disposalService;
        }

        public List<ITrigger> Create(WorkflowStepData param, WorkflowPartData[] parts)
        {
            List<ITrigger> triggerList = new List<ITrigger>();
            if (param.automatic)
            {
                triggerList.Add(CreateAutoTrigger(param));
            }
            else if (param.mount)
            {
                triggerList.Add(CreateMountTrigger(param));
            }
            else if (param.unmount)
            {
                triggerList.Add(CreateEndAssemblyTrigger(param));
            }
            else if (param.control)
            {
                triggerList.Add(CreateAutoTrigger(param));
            }
            else if(param.partId != -1)
            {
                if (param.gripperId != -1)
                {
                    triggerList.Add(CreateGripperPartTrigger(param, parts));
                }
                else
                {
                    triggerList.Add(CreateManualPartTrigger(param, parts));
                }
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
            condition.partId = param.partId;
            trigger.AddCondition(condition, true);
            trigger.advance = true;
            trigger.mode = ConditionMode.and;
            m_DisposalService.Register(trigger, param.id);
            m_DisposalService.Register(condition, param.id);
            return trigger;
        }

        private ITrigger CreateMountTrigger(WorkflowStepData param)
        {
            WorkflowStepTrigger trigger = m_Instantiator.Instantiate<WorkflowStepTrigger>();
            MountCondition condition = m_Instantiator.Instantiate<MountCondition>();
            trigger.AddCondition(condition, true);
            trigger.advance = true;
            trigger.mode = ConditionMode.and;
            m_DisposalService.Register(trigger, param.id);
            m_DisposalService.Register(condition, param.id);
            return trigger;
        }

        private ITrigger CreateEndAssemblyTrigger(WorkflowStepData param)
        {
            WorkflowStepTrigger trigger = m_Instantiator.Instantiate<WorkflowStepTrigger>();
            EndAssemblyCondition condition = m_Instantiator.Instantiate<EndAssemblyCondition>();
            trigger.AddCondition(condition, true);
            trigger.advance = true;
            trigger.mode = ConditionMode.and;
            m_DisposalService.Register(trigger, param.id);
            m_DisposalService.Register(condition, param.id);
            return trigger;
        }

        private ITrigger CreateGripperPartTrigger(WorkflowStepData param, WorkflowPartData[] parts)
        {
            WorkflowStepTrigger trigger = m_Instantiator.Instantiate<WorkflowStepTrigger>();
            GripperPartCondition condition = m_Instantiator.Instantiate<GripperPartCondition>();
            WorkflowPartData part = parts.Where(p => p.id == param.partId).FirstOrDefault();
            condition.stepId = param.id;
            condition.partId = param.partId;
            condition.gripperId = param.gripperId;
            condition.partPosition = part.part_position;
            condition.pcbPosition = part.pcb_position;
            condition.partHeight = part.partHeight;
            condition.Initialize();
            trigger.AddCondition(condition, true);
            trigger.advance = true;
            trigger.mode = ConditionMode.and;
            m_DisposalService.Register(trigger, param.id);
            m_DisposalService.Register(condition, param.id);
            return trigger;
        }

        private ITrigger CreateManualPartTrigger(WorkflowStepData param, WorkflowPartData[] parts)
        {
            WorkflowStepTrigger trigger = m_Instantiator.Instantiate<WorkflowStepTrigger>();
            ManualPartCondition condition = m_Instantiator.Instantiate<ManualPartCondition>();
            WorkflowPartData part = parts.Where(p => p.id == param.partId).FirstOrDefault();
            condition.stepId = param.id;
            condition.partId = param.partId;
            condition.gripperId = param.gripperId;
            condition.partPosition = part.part_position;
            condition.pcbPosition = part.pcb_position;
            condition.partHeight = part.partHeight;
            trigger.AddCondition(condition, true);
            trigger.advance = true;
            trigger.mode = ConditionMode.and;
            m_DisposalService.Register(trigger, param.id);
            m_DisposalService.Register(condition, param.id);
            return trigger;
        }
    }
}