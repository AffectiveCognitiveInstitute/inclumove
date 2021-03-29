using Aci.UI.Binding;
using Aci.Unity.Audio;
using Aci.Unity.Data;
using Aci.Unity.Gamification;
using Aci.Unity.UI.Navigation;
using Aci.Unity.Util;
using Aci.Unity.Workflow;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.UserInterface
{
    public class ReviewScreenPage01ViewController : MonoBindable, INavigatingAware, INavigatedAware
    {
        private int m_CircuitsCompleted;
        private int m_MaxCircuits;
        private int m_TotalBadges;
        private ITimeProvider m_TimeProvider;
        private IWorkflowService m_WorkflowService;
        private IBadgeService m_BadgeService;
        private IUserProfile m_UserProfile;
        private IUserManager m_UserManager;
        private IAudioService m_AudioService;
        private int m_HoursWorked;
        private int m_MinutesWorked;
        private float m_NormalizedCompletedCircuits;

        [SerializeField]
        private AudioClip m_ReviewClip01;

        [SerializeField]
        private AudioClip m_ReviewClip02;

        /// <summary>
        ///     The normalized amount of completed circuits.
        /// </summary>
        public float normalizedCompletedCircuits
        {
            get => m_NormalizedCompletedCircuits;
            set => SetProperty(ref m_NormalizedCompletedCircuits, value);
        }

        /// <summary>
        ///     The maximum number of circuits that can be completed in the workflow.
        /// </summary>
        public int maximumCircuitsCount
        {
            get => m_MaxCircuits;
            set => SetProperty(ref m_MaxCircuits, value);
        }

        /// <summary>
        ///     The total number of circuits (repetitions) that were completed.
        /// </summary>
        public int circuitsCompleted
        {
            get => m_CircuitsCompleted;
            set => SetProperty(ref m_CircuitsCompleted, value);
        }

        /// <summary>
        ///     The total number of hours worked.
        /// </summary>
        public int hoursWorked
        {
            get => m_HoursWorked;
            set => SetProperty(ref m_HoursWorked, value);
        }

        /// <summary>
        ///     The number of minutes (of the current hour) worked.
        /// </summary>
        public int minutesWorked
        {
            get => m_MinutesWorked;
            set => SetProperty(ref m_MinutesWorked, value);
        }

        /// <summary>
        ///     The total number of badges accumulated.
        /// </summary>
        public int totalBadges
        {
            get => m_TotalBadges;
            set => SetProperty(ref m_TotalBadges, value);
        }

        [Zenject.Inject]
        private void Construct(ITimeProvider timeProvider,
                               IWorkflowService workflowService,
                               IBadgeService badgeService,
                               IUserProfile userProfile,
                               IUserManager userManager,
                               IAudioService audioService)
        {
            m_TimeProvider = timeProvider;
            m_WorkflowService = workflowService;
            m_BadgeService = badgeService;
            m_UserProfile = userProfile;
            m_UserManager = userManager;
            m_AudioService = audioService;
        }

        public async void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            maximumCircuitsCount = m_BadgeService.amountLevels.ElementAt(2); 
            circuitsCompleted = m_WorkflowService.currentRepetition;
            totalBadges = m_BadgeService.currentBadges.GetWeightedTotalCount();
            hoursWorked = m_TimeProvider.elapsedTotal.Hours;
            minutesWorked = m_TimeProvider.elapsedTotal.Minutes;

            // Add current badge data to history
            m_UserProfile.AddBadgeDataToHistory(m_BadgeService.currentBadges);
            await m_AudioService.PlayAudioClipAsync(m_ReviewClip01, AudioChannels.Assistant);
            m_AudioService.PlayAudioClip(m_ReviewClip02, AudioChannels.Assistant);
        }

        public void OnNavigatingTo(INavigationParameters navigationParameters)
        {
            normalizedCompletedCircuits = 1- (float)m_WorkflowService.currentRepetition / m_BadgeService.amountLevels.ElementAt(2);
        }
    }
}