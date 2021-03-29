namespace Aci.Unity.Quests
{
    public interface IQuestFacade
    {
        /// <summary>
        /// Gets the instance of quest.
        /// </summary>
        /// <param name="questId">The quest id.</param>
        /// <returns>Returns an instance of <see cref="Quest"/></returns>
        Quest GetInstance(string questId);

        /// <summary>
        /// Returns the current quest state of the given quest id.
        /// </summary>
        /// <param name="questId">The quest id.</param>
        /// <returns>Returns <see cref="QuestState"/> of the given quest id.</returns>
        QuestState GetQuestState(string questId);

        /// <summary>
        /// Starts the given quest.
        /// </summary>
        /// <param name="quest">The quest to be started.</param>
        /// <returns>Returns a copy of the Quest.</returns>
        Quest StartQuest(Quest quest);

        /// <summary>
        /// Stops an instance of the given quest id.
        /// </summary>
        /// <param name="questId">The quest id to be stopped.</param>
        void StopQuest(string questId);
    }
}