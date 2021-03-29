using Aci.UI.Binding;
using Aci.Unity.Audio;
using Aci.Unity.Data;
using Aci.Unity.Gamification;
using Aci.Unity.UI.Navigation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.UserInterface
{
    public class ReviewScreenPage03ViewController : MonoBindable, INavigatedAware
    {
        private RewardCollection m_RewardCollection;

        private INavigationService m_NavigationService;
        private IAudioService m_AudioService;
        private IUserProfile m_UserProfile;
        private RewardViewController.Factory m_Factory;
        private List<RewardViewController> m_ViewControllers = new List<RewardViewController>();

        [SerializeField]
        private AudioClip m_ReviewClip;

        [Zenject.Inject]
        private void Construct(INavigationService navigationService
            , RewardCollection collection
            , RewardViewController.Factory factory
            , IUserProfile userProfile
            , IAudioService audioService)
        {
            m_NavigationService = navigationService;
            m_RewardCollection = collection;
            m_AudioService = audioService;
            m_UserProfile = userProfile;
            m_Factory = factory;
        }

        private void Awake()
        {
            Populate();
        }

        private void Populate()
        {
            // TODO: Check if item is locked/unlocked
            foreach (RewardData rewardData in m_RewardCollection)
            {
                if (!rewardData.unlocked)
                    continue;
                m_ViewControllers.Add(m_Factory.Create(rewardData));
            }
        }

        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            m_AudioService.PlayAudioClip(m_ReviewClip, AudioChannels.Assistant);
        }

        public void OnContinueButtonClicked()
        {
            RewardData selectedReward = m_ViewControllers.FirstOrDefault(x => x.isSelected).data;
            Debug.Log($"Selected reward: {selectedReward.title}");

            m_UserProfile.selectedReward = selectedReward.id;

            NavigationParameters navigationParameters = new NavigationParameters();
            navigationParameters.Add("Reward", selectedReward.id);
            m_NavigationService.PushWithNewStackAsync("Reward", navigationParameters, AnimationOptions.Asynchronous);
        }
    }
}