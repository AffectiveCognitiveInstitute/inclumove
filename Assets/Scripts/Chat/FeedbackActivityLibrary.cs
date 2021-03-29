using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.Chat
{
    [CreateAssetMenu(menuName = "ScriptableObjects/FeedbackActivityLibary")]
    public class FeedbackActivityLibrary : ScriptableObject
    {
        [SerializeField]
        private List<PredefinedFeedbackActivity> m_Activities = new List<PredefinedFeedbackActivity>();

        public PredefinedFeedbackActivity Get(FeedbackType feedbackType)
        {
            List<PredefinedFeedbackActivity> activities = m_Activities.Where(x => x.feedbackType == feedbackType).ToList();

            if (activities.Count == 0)
                throw new Exception($"Could not find a prefined defined message for target FeedbackType: {feedbackType}");

            int activity = UnityEngine.Random.Range(0, activities.Count);
            return activities[activity];
        }

        public PredefinedFeedbackActivity Get(FeedbackType feedbackType, int tier)
        {
            List<PredefinedFeedbackActivity> activities = m_Activities.Where(x => x.feedbackType == feedbackType && x.feedbackTier == tier).ToList();

            if (activities.Count == 0)
                throw new Exception($"Could not find a prefined defined message for target FeedbackType: {feedbackType}");

            int activity = UnityEngine.Random.Range(0, activities.Count);
            return activities[activity];
        }
    }
}