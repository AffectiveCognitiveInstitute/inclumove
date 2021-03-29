using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.Chat
{
    [CreateAssetMenu(menuName = "ScriptableObjects/IncorrectWorkflowStepActivityLibary")]
    public class IncorrectWorkflowStepActivityLibrary : ScriptableObject
    {
        [SerializeField]
        private List<PredefinedIncorrectStepActivity> m_Activities = new List<PredefinedIncorrectStepActivity>();

        public PredefinedActivity Get(int workflowStepIndex)
        {
            PredefinedIncorrectStepActivity predefinedActivity = m_Activities.FirstOrDefault(x => x.workstepIndex == workflowStepIndex);

            if (predefinedActivity == null)
                throw new Exception($"Could not find a prefined defined message for workflow step index: {workflowStepIndex}");

            return predefinedActivity.activity;
        }
    }
}