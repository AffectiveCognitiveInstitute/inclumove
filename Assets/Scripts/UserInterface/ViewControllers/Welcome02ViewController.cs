using Aci.Unity.Audio;
using Aci.Unity.UI.Navigation;
using Aci.Unity.UI.Tweening;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class Welcome02ViewController : MonoBehaviour, INavigatedAware
    {
        [SerializeField]
        private TweenerDirector m_HelloTweener;

        [SerializeField]
        private TweenerDirector m_PressPlayTweener;

        [SerializeField]
        private TweenerDirector m_ContinueButtonTweener;

        [SerializeField]
        private AudioClip m_GreetingClip;

        [SerializeField]
        private AudioClip m_StartButtonClip;
        
        private IAudioService m_AudioService;

        [Inject]
        private void Construct(IAudioService audioService)
        {
            m_AudioService = audioService;
        }

        public async void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            await PlaySpeechBubbleAudio(m_GreetingClip, m_HelloTweener);
            await m_HelloTweener.PlayReverseAsync();
            m_ContinueButtonTweener.PlayForwardsAsync();
            await PlaySpeechBubbleAudio(m_StartButtonClip, m_PressPlayTweener);
        }

        private async Task PlaySpeechBubbleAudio(AudioClip clip, TweenerDirector tweener)
        {
            float duration = 0;
            foreach(Tweener tween in tweener.tweeners)
            {
                duration = duration < tween.delayTime + tween.duration ? tween.delayTime + tween.duration : duration;
            }
            tweener.PlayForwardsAsync();
            await Task.Delay(Mathf.FloorToInt((duration)*1000)-800);
            await m_AudioService.PlayAudioClipAsync(clip, AudioChannels.Assistant);
        }
    }
}