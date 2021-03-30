using System.Collections.Generic;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Events;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Workflow.Triggers
{
    public class TriggerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindFactory<WorkflowStepData, WorkflowPartData[], List<ITrigger>, WorkflowStepTrigger.Factory>()
                     .FromFactory<WorkflowStepTriggerFactory>();
        }
    }
}