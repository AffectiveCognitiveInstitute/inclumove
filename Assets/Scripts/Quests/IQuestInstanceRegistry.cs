namespace Aci.Unity.Quests
{
    public interface IQuestInstanceRegistry
    {
        void Register(Quest instance);

        void Unregister(Quest instance);

        Quest GetQuestById(string questId);
    }
}
