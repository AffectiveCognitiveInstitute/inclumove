using UnityEngine;

namespace Aci.Unity.Data
{
    public abstract class MilestoneCommand : ScriptableObject
    {
        /// <summary>
        ///     Called when the unlockable became activated.
        /// </summary>
        public abstract void OnBecameActivated();

        /// <summary>
        ///     Called when the unlockable became deactivated.
        /// </summary>
        public abstract void OnBecameDeactivated();
    }
}