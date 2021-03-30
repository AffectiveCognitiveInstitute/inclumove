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
            id = 0,
            name = "NewStep",
            automatic = false,
            mount = false,
            unmount = false,
            control = false,
            partId = -1,
            gripperId = -1,
            levels = 0,
            durations = new float[] {0, 0, 0},
            items = new SceneItemData[] {},
            repetitions = 0,
            currentRepetitions = 0
        };

        public uint id;
        public string name;
        public bool automatic;
        public bool mount;
        public bool unmount;
        public bool control;
        public int partId;
        public int gripperId;
        public byte levels;
        public float[] durations;
        public SceneItemData[] items;
        public int repetitions;
        public int currentRepetitions;
    }
}
