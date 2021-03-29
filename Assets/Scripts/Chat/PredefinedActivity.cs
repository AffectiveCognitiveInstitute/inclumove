using UnityEngine;

namespace Aci.Unity.Chat
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Predefined Activity")]
    public class PredefinedActivity : ScriptableObject
    {
        [SerializeField]
        private string m_Message;

        [SerializeField]
        private bool m_HasImage;

        [SerializeField]
        private string m_Image;

        [SerializeField]
        private SuggestedAction[] m_SuggestedActions;

        /// <summary>
        /// The message text
        /// </summary>
        public string message => m_Message;

        /// <summary>
        /// Does the activity contain an image
        /// </summary>
        public bool hasImage => m_HasImage;

        /// <summary>
        /// The path of the image
        /// </summary>
        public string image => m_Image;

        /// <summary>
        /// Suggested actions that can appear as options for the user.
        /// </summary>
        public SuggestedAction[] suggestedActions => m_SuggestedActions;
    }

    [System.Serializable]
    public struct SuggestedAction
    {
        public string Message;
        public string Type;
    }
}
