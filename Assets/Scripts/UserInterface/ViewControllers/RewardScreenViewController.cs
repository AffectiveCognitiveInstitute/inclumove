using Aci.UI.Binding;
using Aci.Unity.Data;
using Aci.Unity.Gamification;
using Aci.Unity.Gamification.Rewards;
using Aci.Unity.UI.Navigation;
using Aci.Unity.Util;
using UnityEngine;

namespace Aci.Unity.UserInterface
{
    public class RewardScreenViewController : MonoBindable,
                                              INavigatedAware
    {
        [SerializeField]
        private GameObject m_ParticleSystem;
        private INavigationService m_NavigationService;
        private IBadgeService m_BadgeService;
        private RewardParticleSystemFactory m_RewardParticleSystemFactory;

        [Zenject.Inject]
        private void Construct(INavigationService navigationService,
                               RewardParticleSystemFactory rewardParticleSystemFactory,
                               IBadgeService badgeService)
        {
            m_NavigationService = navigationService;
            m_BadgeService = badgeService;
            m_RewardParticleSystemFactory = rewardParticleSystemFactory;
        }


        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            BadgeData data = m_BadgeService.currentBadges;

            string reward = "Balloons";
            navigationParameters.TryGetValue("Reward", out reward);

            m_RewardParticleSystemFactory.Create(reward, new RewardParticleSystemFactoryParams(new SubEmitterParams(data.AmountBadges[0], data.AmountBadges[1], data.AmountBadges[2]),
                                                                                                   new SubEmitterParams(data.TimeBadges[0], data.TimeBadges[1], data.TimeBadges[2]),
                                                                                                   new SubEmitterParams(data.StreakBadges[0], data.StreakBadges[1], data.StreakBadges[2])));
            // Use this for testing:
            /*m_RewardParticleSystemFactory.Create(reward, new RewardParticleSystemFactoryParams(new SubEmitterParams(10, 0, 0),
                                                                                                 new SubEmitterParams(0, 0, 0),
                                                                                                 new SubEmitterParams(0, 0, 0)));*/

            m_ParticleSystem.GetComponent<ParticleSystemStoppedEventBridge>().particleSystemStopped.AddListener(ParticleSystemStopped);
            m_ParticleSystem.GetComponent<ParticleSystemStoppedEventBridge>().PlayAll();
        }

        private void ParticleSystemStopped(ParticleSystem ps)
        {
            m_NavigationService.PushWithNewStackAsync("Farewell01", AnimationOptions.Asynchronous);
        }
    }
}