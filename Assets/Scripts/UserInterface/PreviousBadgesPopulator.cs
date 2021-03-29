using Aci.UI.Binding;
using Aci.Unity.Data;
using Aci.Unity.Gamification;
using Aci.Unity.UserInterface.ViewControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.UserInterface
{
    public class PreviousBadgesPopulator : MonoBindable
    {
        [SerializeField, Tooltip("This will exclude the first day in the list")]
        private bool m_ExcludeLastInHistory = false;

        [SerializeField]
        private uint m_SampleCount = 4;

        private bool m_WasHistoryFound;
        private IUserProfile m_UserProfile;
        private PreviousBadgeViewController.Factory m_Factory;

        public bool wasHistoryFound
        {
            get => m_WasHistoryFound;
            set => SetProperty(ref m_WasHistoryFound, value);
        }

        [Zenject.Inject]
        private void Construct(IUserProfile userProfile, PreviousBadgeViewController.Factory factory)
        {
            m_UserProfile = userProfile;
            m_Factory = factory;
        }

        private void Awake()
        {
            wasHistoryFound = m_UserProfile.badges.Count > 0;

            if (!wasHistoryFound)
                return;

            // Create a tupled list
            List<(DateTime dateTime, BadgeData badgeData)> history = new List<(DateTime, BadgeData)>();
            for(int i = 0; i < m_UserProfile.badges.Count; i++)
                history.Add((m_UserProfile.badgeDates[i], m_UserProfile.badges[i]));

            // Sort from new to old
            history.Sort((x, y) => y.dateTime.CompareTo(x.dateTime));

            if (m_ExcludeLastInHistory)
                history.RemoveAt(0);

            // Take sample count
            history = history.Take((int)m_SampleCount).ToList();

            // Populate...
            for (int i = history.Count-1; i >= 0; i--)
                m_Factory.Create(history[i].dateTime, history[i].badgeData);
        }
    }
}