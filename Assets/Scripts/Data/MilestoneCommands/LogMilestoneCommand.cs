using UnityEngine;

namespace Aci.Unity.Data
{
    [CreateAssetMenu(menuName = "Inclumove/Milestone Commands/Log Command")]
    public class LogMilestoneCommand : MilestoneCommand
    {
        public override void OnBecameActivated()
        {
            Debug.Log("Milestone Unlockable became activated.");
        }

        public override void OnBecameDeactivated()
        {
            Debug.Log("Milestone Unlockable became deactivated.");
        }
    }
}