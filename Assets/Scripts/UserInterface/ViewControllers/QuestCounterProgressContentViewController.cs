using Unity.Mathematics;

namespace Aci.Unity.Quests
{
    public class QuestCounterProgressContentViewController : QuestContentViewController
    {
        private QuestProgressContent m_Content;

        private float m_Progress;
        public float progress => math.saturate((float) m_Content.currentValue / m_Content.targetValue);

        private string m_TitleKey;
        public string titleKey
        {
            get => m_TitleKey;
            set => SetProperty(ref m_TitleKey, value);
        }

        private string m_CountText;
        public string countText
        {
            get => m_CountText;
            set => SetProperty(ref m_CountText, value);
        }

        [Zenject.Inject]
        private void Construct(QuestContent content)
        {
            m_Content = content as QuestProgressContent;
            titleKey = m_Content.titleKey;
            countText = $"{m_Content.currentValue} / {m_Content.targetValue}";
        }
    }
}