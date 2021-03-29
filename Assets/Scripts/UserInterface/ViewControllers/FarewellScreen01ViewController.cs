using Aci.UI.Binding;
using Aci.Unity.Audio;
using Aci.Unity.Events;
using Aci.Unity.Gamification;
using Aci.Unity.UI.Navigation;
using Aci.Unity.UI.Tweening;
using Aci.Unity.Util;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Aci.Unity.UserInterface
{
    public class FarewellScreen01ViewController : MonoBindable, INavigatedAware, IAciEventHandler<USBDevice>
    {
        [SerializeField]
        private TweenerDirector m_WaitForDataMessageTweener;

        [SerializeField]
        private TweenerDirector m_RemoveStickMessageTweener;

        [SerializeField]
        private AudioClip m_WaitForDataClip;

        [SerializeField]
        private AudioClip m_RemovalClip;

        [SerializeField]
        private Animator m_FemosAnimator;

        private INavigationService m_NavigationService;
        private IUserManager m_UserManager;
        private IConfigProvider m_ConfigProvider;
        private IAciEventManager m_EventManager;
        private IAudioService m_AudioService;
        private UsbDetectorService m_UsbService;

        [ConfigValue("UseUsbProfile")]
        public bool UseUsbProfile { get; set; }


        [Zenject.Inject]
        private void Construct(INavigationService navigationService,
                               IUserManager userManager,
                               IConfigProvider configProvider,
                               IAciEventManager eventManager,
                               IAudioService audioService,
                               UsbDetectorService usbService)
        {
            m_NavigationService = navigationService;
            m_UserManager = userManager;
            m_ConfigProvider = configProvider;
            m_EventManager = eventManager;
            m_AudioService = audioService;
            m_UsbService = usbService;
        }

        private void OnEnable()
        {
            m_UsbService.active = true;
            m_ConfigProvider.RegisterClient(this);
        }

        private void OnDisable()
        {
            m_ConfigProvider.UnregisterClient(this);
        }

        private async Task PlaySpeechBubbleAudio(AudioClip clip, TweenerDirector tweener, bool awaitExit)
        {
            float duration = 0;
            foreach (Tweener tween in tweener.tweeners)
            {
                duration = duration < tween.delayTime + tween.duration ? tween.delayTime + tween.duration : duration;
            }
            tweener.PlayForwardsAsync();
            await Task.Delay(Mathf.FloorToInt((duration) * 1000) - 800);
            if (awaitExit)
                await m_AudioService.PlayAudioClipAsync(clip, AudioChannels.Assistant);
            else
                m_AudioService.PlayAudioClip(clip, AudioChannels.Assistant);
        }

        public async void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            m_FemosAnimator.SetTrigger("Speech");
            await PlaySpeechBubbleAudio(m_WaitForDataClip, m_WaitForDataMessageTweener, false);
            try
            {
                bool result = await m_UserManager.SaveUser();

                // TODO: Handle case where user didn't save successfully. Display Popup?
                if (result)
                    Debug.Log("Save User completed successfully");
                else
                    Debug.Log("Save User failed");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                if (UseUsbProfile)
                {
                    RegisterForEvents();
                    await Task.Delay(TimeSpan.FromSeconds(m_WaitForDataClip.length - 0.8f));
                    await m_WaitForDataMessageTweener.PlayReverseAsync();
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    m_FemosAnimator.SetTrigger("Speech");
                    await PlaySpeechBubbleAudio(m_RemovalClip, m_RemoveStickMessageTweener, true);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(m_WaitForDataClip.length - 0.8f));
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    await m_WaitForDataMessageTweener.PlayReverseAsync();
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    m_FemosAnimator.SetTrigger("Speech");
                    await PlaySpeechBubbleAudio(m_RemovalClip, m_RemoveStickMessageTweener, true);
                    await m_RemoveStickMessageTweener.PlayReverseAsync();
                    await m_NavigationService.PushWithNewStackAsync("Farewell02", AnimationOptions.Asynchronous);
                }
                m_UsbService.active = false;
            }
        }

        public void RegisterForEvents()
        {
            m_EventManager.AddHandler(this);
        }

        public void UnregisterFromEvents()
        {
            m_EventManager.RemoveHandler(this);
        }

        public async void OnEvent(USBDevice eventArgs)
        {
            if(string.IsNullOrEmpty(eventArgs.drive))
            {
                UnregisterFromEvents();
                await Task.Delay(TimeSpan.FromSeconds(2));
                await m_RemoveStickMessageTweener.PlayReverseAsync();
                await m_NavigationService.PushWithNewStackAsync("Farewell02", AnimationOptions.Asynchronous);
            }
        }
    }
}