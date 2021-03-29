using Aci.UI.Binding;
using Aci.Unity.Data;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Aci.Unity.UserInterface
{
    public class RewardViewController : MonoBindable
    {
        public class Factory : PlaceholderFactory<RewardData, RewardViewController> { }

        private string m_Title;
        public string title
        {
            get => m_Title;
            set => SetProperty(ref m_Title, value);
        }

        private Sprite m_Icon;
        public Sprite icon
        {
            get => m_Icon;
            set => SetProperty(ref m_Icon, value);
        }

        public bool isSelected { get; set; } // set through inspector

        public RewardData data { get; private set; }

        [Zenject.Inject]
        private void Construct(RewardData rewardData)
        {
            data = rewardData;
            title = rewardData.title;
            icon = rewardData.icon;
        }

        private void Start()
        {
            GetComponent<Toggle>().group = GetComponentInParent<ToggleGroup>();
        }
    }
}