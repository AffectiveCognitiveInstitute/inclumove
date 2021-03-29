using UnityEngine;

namespace Aci.Unity.Chat
{
    [System.Serializable]
    public class PredefinedIncorrectStepActivity
    {
        [SerializeField]
        private int m_WorkStepIndex;

        [SerializeField]
        private PredefinedActivity m_PredefinedActivity;

        /// <summary>
        /// The workflow step index the message corresponds to
        /// </summary>
        public int workstepIndex => m_WorkStepIndex;

        /// <summary>
        /// The message to be displayed
        /// </summary>
        public PredefinedActivity activity => m_PredefinedActivity;
    }
}
