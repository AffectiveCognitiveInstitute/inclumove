using Aci.UI.Binding;
using Aci.Unity.Data;
using Aci.Unity.Gamification;
using Aci.Unity.UI.Dialog;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class MilestoneViewController : MonoBindable
    {
        public class Factory : PlaceholderFactory<MilestoneData, bool, MilestoneViewController> { }

        [SerializeField]
        private ColorScheme m_LockedScheme;

        private MilestoneData m_Data;
        private bool m_IsUnlocked;
        public bool isUnlocked
        {
            get => m_IsUnlocked;
            set => SetProperty(ref m_IsUnlocked, value);
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

        private Sprite m_Icon;
        public Sprite icon
        {
            get => m_Icon;
            set => SetProperty(ref m_Icon, value);
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

        private bool m_IsUnlockableActivated;
        public bool isUnlockableActivated
        {
            get => m_IsUnlockableActivated;
            set => SetProperty(ref m_IsUnlockableActivated, value);
        }


        private IDialogService m_DialogService;
        private MilestoneDialogueViewController.Factory m_DialogFactory;
        private IUserProfile m_UserProfile;
        private bool m_IsBusy;

        [Zenject.Inject]
        private void Construct(MilestoneData data,
                               bool isUnlocked,
                               IDialogService dialogService,
                               IUserProfile userProfile,
                               MilestoneDialogueViewController.Factory dialogFactory)
        {
            m_Data = data;
            m_IsUnlocked = isUnlocked;
            title = data.title;
            subtitle = data.subtitle;
            icon = data.icon;
            m_DialogService = dialogService;
            m_DialogFactory = dialogFactory;
            m_UserProfile = userProfile;
            isUnlockableActivated = m_UserProfile.IsMilestoneActivated(data.id);

            if (data.colorScheme != null)
            {
                colorA = !isUnlocked? m_LockedScheme.primaryColor : data.colorScheme.primaryColor;
                colorB = !isUnlocked? m_LockedScheme.secondaryColor : data.colorScheme.secondaryColor;
            }
            else
            {
                colorA = m_LockedScheme.primaryColor;
                colorB = m_LockedScheme.secondaryColor;
            }
        }

        public async void OnClick()
        {
            if (m_IsBusy)
                return;

            m_IsBusy = true;
            try
            {
                DialogComponent vc = m_DialogFactory.Create(m_Data);
                m_DialogService.SendRequest(DialogRequest.Create(vc));
                vc.dismissed += (dialog) => isUnlockableActivated = m_UserProfile.IsMilestoneActivated(m_Data.id);
                await Task.Delay(200);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                m_IsBusy = false;
            }
        }
    }
}