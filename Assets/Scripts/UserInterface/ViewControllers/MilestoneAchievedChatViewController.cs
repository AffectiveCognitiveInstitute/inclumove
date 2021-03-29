using Aci.UI.Binding;
using Aci.Unity.Data;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Chat
{
    public class MilestoneAchievedChatViewController : MonoBindable
    {
        public class Factory : PlaceholderFactory<string, MilestoneData, MilestoneAchievedChatViewController> { }

        private string m_Title;
        public string title
        {
            get => m_Title;
            set => SetProperty(ref m_Title, value);
        }

        private string m_Subtitle;
        public string subtitle
        {
            get => m_Subtitle;
            set => SetProperty(ref m_Subtitle, value);
        }

        private Sprite m_Icon;
        public Sprite icon
        {
            get => m_Icon;
            set => SetProperty(ref m_Icon, value);
        }

        private Color m_ColorA;
        public Color colorA
        {
            get => m_ColorA;
            set => SetProperty(ref m_ColorA, value);
        }

        private Color m_ColorB;
        public Color colorB
        {
            get => m_ColorB;
            set => SetProperty(ref m_ColorB, value);
        }

        private string m_Message;
        public string message
        {
            get => m_Message;
            set => SetProperty(ref m_Message, value);
        }

        [Zenject.Inject]
        private void Construct(string message, MilestoneData milestoneData)
        {
            title = milestoneData.title;
            subtitle = milestoneData.subtitle;
            icon = milestoneData.icon;
            colorA = milestoneData.colorScheme.primaryColor;
            colorB = milestoneData.colorScheme.secondaryColor;
            this.message = message;
        }
    }
}