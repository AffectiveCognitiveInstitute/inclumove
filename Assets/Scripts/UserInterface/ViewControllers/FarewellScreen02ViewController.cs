using Aci.Unity.Audio;
using Aci.Unity.Gamification;
using Aci.Unity.UI.Navigation;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Aci.Unity.UserInterface
{
    public class FarewellScreen02ViewController : MonoBehaviour, INavigatedAware
    {
        [SerializeField]
        private AudioClip m_FarewellClip;

        [SerializeField]
        private float m_NextScreenDelay = 5f;
        private INavigationService m_NavigationService;
        private IUserManager m_UserManager;
        private IAudioService m_AudioService;

        [Zenject.Inject]
        private void Construct(INavigationService navigationService, IUserManager userManager, IAudioService audioService)
        {
            m_NavigationService = navigationService;
            m_UserManager = userManager;
            m_AudioService = audioService;
        }

        public async void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            m_AudioService.PlayAudioClip(m_FarewellClip, AudioChannels.Assistant);
            await Task.Delay(TimeSpan.FromSeconds(m_NextScreenDelay));
            m_UserManager.CurrentUser = null;
            await m_NavigationService.PushWithNewStackAsync("Idle", AnimationOptions.Synchronous);
        }
    }
}