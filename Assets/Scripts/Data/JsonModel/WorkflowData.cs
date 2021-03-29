using System;
using System.Collections.Generic;
using Aci.Unity.Scene.SceneItems;
using Zenject;

namespace Aci.Unity.Data.JsonModel
{
    [Serializable]
    public struct WorkflowData : IEquatable<WorkflowData>
    {
        public class Factory : PlaceholderFactory<List<IStepItem>, WorkflowData> { }

        public static WorkflowData Empty = new WorkflowData() {name = "", numTotalSteps = 0, steps = null};
        public string name;
        public WorkflowStepData[] steps;
        public int numTotalSteps;

        public bool Equals(WorkflowData other) => this.name == other.name &&
                                                  this.numTotalSteps == other.numTotalSteps &&
                                                  this.steps == other.steps;

        public static bool operator ==(WorkflowData first, WorkflowData second)
        {
            return Equals(first, second);
        }
        public static bool operator !=(WorkflowData first, WorkflowData second)
        {
            return !(first == second);
        }
    }
}
