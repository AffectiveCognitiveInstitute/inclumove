using UnityEngine;

namespace Aci.Unity.Chat
{
    [CreateAssetMenu(menuName = "ScriptableObjects/AdaptivityActivityLibrary")]
    public class AdaptivityActivityLibrary : ScriptableObject
    {
        [SerializeField]
        private AdaptivityActivityMessage[] m_IncreaseAdaptivityLevelMessages;

        [SerializeField]
        private AdaptivityActivityMessage[] m_DecreaseAdaptivityLevelMessages;

        public AdaptivityActivityMessage IncreaseAdaptivityLevelMessage
        {
            get => m_IncreaseAdaptivityLevelMessages[Random.Range(0, m_IncreaseAdaptivityLevelMessages.Length)];
        }

        public AdaptivityActivityMessage DecreaseAdaptivityLevelMessage
        {
            get => m_DecreaseAdaptivityLevelMessages[Random.Range(0, m_DecreaseAdaptivityLevelMessages.Length)];
        }
    }

    [System.Serializable]
    public class AdaptivityActivityMessage
    {
        public PredefinedActivity activity;
        public string audioClip;
    }
}
