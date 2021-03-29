using Aci.UI.Binding;
using Aci.Unity.Data;
using Aci.Unity.Gamification;
using Aci.Unity.Quests;
using Aci.Unity.UI.Dialog;
using Aci.Unity.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    [RequireComponent(typeof(DialogComponent))]
    public class MilestoneDialogueViewController : MonoBindable
    {
        [SerializeField]
        private RectTransform m_QuestContentParent;

        [SerializeField]
        private Slider m_MilestoneActivatedSlider;

        private MilestoneData m_MilestoneData;
        private IMilestoneFacade m_MilestoneFacade;
        private IUserManager m_UserManager;
        private DialogComponent m_DialogComponent;

        public class Factory : PlaceholderFactory<MilestoneData, MilestoneDialogueViewController> { }

        private Sprite m_Icon;
        public Sprite icon
        {
            get => m_Icon;
            set => SetProperty(ref m_Icon, value);
        }

        private string m_Title;
        public string title
        {
            get => m_Title;
            set => SetProperty(ref m_Title, value);
        }

        private string m_Subtitle;
        public string subtitle
        {
            get => m_Subtitle;
            set => SetProperty(ref m_Subtitle, value);
        }

        private bool m_IsQuestCompleted;
        public bool isQuestCompleted
        {
            get => m_IsQuestCompleted;
            set => SetProperty(ref m_IsQuestCompleted, value);
        }

        private bool m_HasUnlockableContent;
        public bool hasUnlockableContent
        {
            get => m_HasUnlockableContent;
            set => SetProperty(ref m_HasUnlockableContent, value);
        }

        private bool m_IsUnlockableActivated;
        public bool isUnlockableActivated
        {
            get => m_IsUnlockableActivated;
            set
            {
                SetProperty(ref m_IsUnlockableActivated, value, () =>
                {
                    if (m_IsUnlockableActivated)
                        m_MilestoneFacade.ActivateUnlockable(m_MilestoneData.id);
                    else
                        m_MilestoneFacade.DeactivateUnlockable(m_MilestoneData.id);
                });
            }
        }

        private Sprite m_UnlockablePreview;
        public Sprite unlockablePreview
        {
            get => m_UnlockablePreview;
            set => SetProperty(ref m_UnlockablePreview, value);
        }

        private Color m_ColorA;
        public Color colorA
        {
            get => m_ColorA;
            set => SetProperty(ref m_ColorA, value);
        }

        private Color m_ColorB;
        public Color colorB
        {
            get => m_ColorB;
            set => SetProperty(ref m_ColorB, value);
        }

        private MilestoneDialogContentFactory m_Factory;
        private MilestoneCommand m_Command;
        private IQuestFacade m_QuestFacade;

        [Inject]
        private void Construct(MilestoneData milestoneData,
                               IQuestFacade questFacade,
                               MilestoneDialogContentFactory factory,
                               IMilestoneFacade milestoneFacade,
                               IUserManager userManager)
        {
            m_MilestoneData = milestoneData;
            m_MilestoneFacade = milestoneFacade;
            m_UserManager = userManager;

            icon = milestoneData.icon;
            title = milestoneData.title;
            subtitle = milestoneData.subtitle;
            if (milestoneData.requiredQuest != null)
                isQuestCompleted = questFacade.GetQuestState(milestoneData.requiredQuest.id) == QuestState.Success;

            hasUnlockableContent = milestoneData.hasUnlockableContent;
            if (hasUnlockableContent)
            {
                m_IsUnlockableActivated = m_UserManager.CurrentUser.IsMilestoneActivated(milestoneData.id);
                m_MilestoneActivatedSlider.value = m_IsUnlockableActivated ? 1f : 0f;
                unlockablePreview = milestoneData.unlockable.preview;
            }
            colorA = milestoneData.colorScheme.primaryColor;
            colorB = milestoneData.colorScheme.secondaryColor;


            m_QuestFacade = questFacade;
            m_Factory = factory;
        }

        private void Awake()
        {
            m_DialogComponent = GetComponent<DialogComponent>();

            if (m_MilestoneData.requiredQuest != null)
            {
                Quest instance = m_QuestFacade.GetInstance(m_MilestoneData.requiredQuest.id);
                foreach (QuestContent content in instance.contents)
                    m_Factory.Create(m_QuestContentParent, content);
            }
        }

        private void OnEnable()
        {
            m_DialogComponent.dismissed += OnDismissed;
        }

        private void OnDisable()
        {
            m_DialogComponent.dismissed -= OnDismissed;
        }

        private void OnDismissed(IDialog dialog)
        {
            Destroy(gameObject);
        }

        public void OnSliderValueChanged(float value)
        {
            isUnlockableActivated = value == 1.0f;
        }

        public static implicit operator DialogComponent(MilestoneDialogueViewController vc) => vc.m_DialogComponent;
    }
}