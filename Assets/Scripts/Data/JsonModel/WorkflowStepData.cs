using System;
using Aci.Unity.Scene.SceneItems;
using Zenject;

namespace Aci.Unity.Data.JsonModel
{
    [Serializable]
    public struct WorkflowStepData
    {
        public class Factory : PlaceholderFactory<IStepItem, WorkflowStepData> { }

        public static WorkflowStepData Empty = new WorkflowStepData()
        {
            id = Guid.Empty.ToString("N"),
            name = "NewStep",
            automatic = false,
            triggerId = -1,
            levels = 0,
            durations = new float[] {0, 0, 0},
            items = new SceneItemData[] {},
            repetitions = 0,
            currentRepetitions = 0
        };

        public string id;
        public string name;
        public bool automatic;
        public int triggerId;
        public byte levels;
        public float[] durations;
        public SceneItemData[] items;
        public int repetitions;
        public int currentRepetitions;
    }
}
