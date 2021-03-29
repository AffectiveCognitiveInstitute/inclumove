using Aci.UI.Binding;
using Aci.Unity.Audio;
using Aci.Unity.Gamification;
using Aci.Unity.UI.Navigation;
using Aci.Unity.UserInterface.ViewControllers;
using System;
using UnityEngine;

namespace Aci.Unity.UserInterface
{
    public class ReviewScreenPage02ViewController : MonoBindable, INavigatedAware
    {
        [SerializeField]
        private BadgesReviewViewController m_CircuitBadges;
        [SerializeField]
        private BadgesReviewViewController m_SpeedBadges;
        [SerializeField]
        private BadgesReviewViewController m_StreakBadges;
        [SerializeField]
        private DayPerformanceViewController m_TodayViewController;

        [SerializeField]
        private AudioClip m_ReviewClip;

        private int m_TotalBadgesToday;
        private IBadgeService m_BadgeService;
        private IAudioService m_AudioService;

        public int totalBadgesToday
        {
            get => m_TotalBadgesToday;
            set => SetProperty(ref m_TotalBadgesToday, value);
        }

        [Zenject.Inject]
        private void Construct(IBadgeService badgeService, IAudioService audioService)
        {
            m_BadgeService = badgeService;
            m_AudioService = audioService;
        }

        private void Awake()
        {
            m_TodayViewController.Initialize(DateTime.Now.ToLocalTime(), m_BadgeService.currentBadges);
        }

        public void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            m_CircuitBadges.Set(m_BadgeService.currentBadges.AmountBadges[0], m_BadgeService.currentBadges.AmountBadges[1], m_BadgeService.currentBadges.AmountBadges[2],
                () => m_SpeedBadges.Set(m_BadgeService.currentBadges.TimeBadges[0], m_BadgeService.currentBadges.TimeBadges[1], m_BadgeService.currentBadges.TimeBadges[2],
                () => m_StreakBadges.Set(m_BadgeService.currentBadges.StreakBadges[0], m_BadgeService.currentBadges.StreakBadges[1], m_BadgeService.currentBadges.StreakBadges[2],
                () => totalBadgesToday = m_CircuitBadges.totalBadges + m_SpeedBadges.totalBadges + m_StreakBadges.totalBadges)));
            m_AudioService.PlayAudioClip(m_ReviewClip, AudioChannels.Assistant);
        }
    }
}