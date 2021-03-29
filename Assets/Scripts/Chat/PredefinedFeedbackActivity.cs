using UnityEngine;

namespace Aci.Unity.Chat
{
    [System.Serializable]
    public class PredefinedFeedbackActivity
    {
        [SerializeField]
        private FeedbackType m_FeedbackType;

        [SerializeField]
        private int m_Tier = 0;

        [SerializeField]
        private string m_AudioClip;

        [SerializeField]
        private PredefinedActivity m_PredefinedActivity;

        /// <summary>
        /// The FeedbackType the message corresponds to
        /// </summary>
        public FeedbackType feedbackType => m_FeedbackType;

        /// <summary>
        /// The assigned tier of the feedback
        /// </summary>
        public int feedbackTier => m_Tier;

        /// <summary>
        /// The message to be displayed
        /// </summary>
        public PredefinedActivity activity => m_PredefinedActivity;

        /// <summary>
        /// The audio to be played
        /// </summary>
        public string audioClip => m_AudioClip;
    }

    public enum FeedbackType
    {
        assertive,
        supportive,
        neutral,
        amountBadge,
        fastBadge,
        streakBadge
    }
}
