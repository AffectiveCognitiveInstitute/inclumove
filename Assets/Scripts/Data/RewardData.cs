using UnityEngine;

namespace Aci.Unity.Data
{
    [CreateAssetMenu(menuName = "Inclumove/Reward")]
    public class RewardData : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private string m_Title;
        [SerializeField]
        private GameObject m_Prefab;
        [SerializeField]
        private bool m_Unlocked;

        public string id => m_Id;
        public Sprite icon => m_Icon;
        public GameObject prefab => m_Prefab;
        public string title => m_Title;
        public bool unlocked
        {
            get => m_Unlocked;
            set => m_Unlocked = value;
        }
    }
}