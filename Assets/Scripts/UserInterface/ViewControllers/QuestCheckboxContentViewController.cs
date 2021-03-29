namespace Aci.Unity.Quests
{
    public class QuestCheckboxContentViewController : QuestContentViewController
    {
        private HasCompletedQuestNodeContent m_Content;

        public bool isDone => m_Content.isCompleted;

        public string titleKey => m_Content.titleKey;

        [Zenject.Inject]
        private void Construct(QuestContent content)
        {
            m_Content = content as HasCompletedQuestNodeContent;
        }
    }
}