using Aci.Unity.Events;

namespace Aci.Unity.Workflow
{
    public interface IStepFinalizer : IAciEventHandler<WorkflowStepFinalizedArgs>
                                    , IAciEventHandler<WorkflowStepEndedArgs>
    {
        bool finalized { get; } 
    }
}
