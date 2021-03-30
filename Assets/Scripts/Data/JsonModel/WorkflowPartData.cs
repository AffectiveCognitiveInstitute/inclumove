using System;
using Aci.Unity.Models;
using Aci.Unity.Scene.SceneItems;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Data.JsonModel
{
    [Serializable]
    public struct WorkflowPartData
    {
        public static WorkflowPartData Empty = new WorkflowPartData() {
            id = 0,
            pcb_position = new PCB_Position { X = 0, Y = 0, PSI = 0 },
            part_position = new PCB_Position { X = 0, Y = 0, PSI = 0 },
            psi = 0,
            partHeight = 0
        };

        public int id;
        public PCB_Position pcb_position;
        public PCB_Position part_position;
        public float psi;
        public float partHeight;
    }
}
