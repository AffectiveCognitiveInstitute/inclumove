using Aci.Unity.Data;
using Aci.Unity.Gamification;
using Aci.Unity.Gamification.Rewards;
using Aci.Unity.UI.Navigation;
using Aci.Unity.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class Welcome01ViewController : MonoBehaviour, INavigatedAware
    {
        [SerializeField]
        private ParticleSystem m_ParticleSystem;

        private INavigationService m_NavigationService;
        private RewardParticleSystemFactory m_RewardParticleSystemFactory;
        private IUserProfile m_UserProfile;

        [Zenject.Inject]
        private void Construct(INavigationService navigationService,
                               RewardParticleSystemFactory rewardParticleSystemFactory,
                               IUserProfile userProfile)
        {
            m_NavigationService = navigationService;
            m_RewardParticleSystemFactory = rewardParticleSystemFactory;
            m_UserProfile = userProfile;
        }

        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            // Create a tupled list
            List<(DateTime dateTime, BadgeData badgeData)> history = new List<(DateTime, BadgeData)>();
            for (int i = 0; i < m_UserProfile.badges.Count; i++)
                history.Add((m_UserProfile.badgeDates[i], m_UserProfile.badges[i]));

            // Sort from new to old
            history.Sort((x, y) => y.dateTime.CompareTo(x.dateTime));

            BadgeData data = history.FirstOrDefault().badgeData;
            
            m_RewardParticleSystemFactory.Create(m_UserProfile.selectedReward, new RewardParticleSystemFactoryParams(new SubEmitterParams(data.AmountBadges.x, data.AmountBadges.y, data.AmountBadges.z),
                                                                                                   new SubEmitterParams(data.TimeBadges.x, data.TimeBadges.y, data.TimeBadges.z),
                                                                                                   new SubEmitterParams(data.StreakBadges.x, data.StreakBadges.y, data.StreakBadges.z)));

            m_ParticleSystem.GetComponent<ParticleSystemStoppedEventBridge>().particleSystemStopped.AddListener(ParticleSystemStopped);
            m_ParticleSystem.GetComponent<ParticleSystemStoppedEventBridge>().PlayAll();
        }

        private async void ParticleSystemStopped(ParticleSystem arg0)
        {
            await Task.Delay(120);
            m_NavigationService.PushAsync("Welcome02", AnimationOptions.Synchronous, false);
        }
    }
}