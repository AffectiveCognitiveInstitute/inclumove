using Aci.UI.Binding;
using Aci.Unity.Gamification;
using UnityEngine;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class UserProfileViewController : MonoBindable
    {
        private string m_UserName;
        private Sprite m_ProfileImage;

        public string userName
        {
            get => m_UserName;
            set => SetProperty(ref m_UserName, value);
        }

        public Sprite profileImage
        {
            get => m_ProfileImage;
            set => SetProperty(ref m_ProfileImage, value);
        }

        [Zenject.Inject]
        public void Construct(IUserProfile userProfile)
        {
            if (userProfile == null)
                return;

            userName = userProfile.userName;
            profileImage = userProfile.userPicture;
        }
    }
}

