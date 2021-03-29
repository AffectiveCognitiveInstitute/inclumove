using Aci.Unity.Data;
using Aci.Unity.UI.Dialog;

namespace Aci.Unity.Util
{
    public interface IMilestoneFacade 
    {
        /// <summary>
        /// Starts all milestone quests.
        /// </summary>
        void StartMilestoneQuests();

        /// <summary>
        /// Stops all milestone quests.
        /// </summary>
        void StopMilestoneQuests();

        /// <summary>
        /// Displays a notification of a milestone that was achieved.
        /// </summary>
        /// <param name="milestoneData">The milestone that was achieved.</param>
        /// <returns>Returns an instance of <see cref="IDialog"/>.</returns>
        IDialog DisplayMilestoneAchieved(MilestoneData milestoneData);

        /// <summary>
        /// Activates the unlockable of the given milestone.
        /// </summary>
        /// <param name="id">The id of the milestone.</param>
        /// <param name="storeChanges">Should the changes be applied immediately to the UserProfile?</param>
        void ActivateUnlockable(string id, bool storeChanges = true);

        /// <summary>
        /// Deactivates the unlockable.
        /// </summary>
        /// <param name="id">The id of the milestone.</param>
        /// <param name="storeChanges">Should the changes be applied immediately to the UserProfile?</param>
        void DeactivateUnlockable(string id, bool storeChanges = true);
    }
}