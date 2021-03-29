using Aci.Unity.UI;
using Aci.Unity.UserInterface.Animation;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Aci.Unity.UserInterface.ViewControllers
{
    [RequireComponent(typeof(Button))]
    public class ActionSheetOptionViewController : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_ImageHolder;

        [SerializeField]
        private CachedImageComponent m_CachedImage;

        [SerializeField]
        private TMPro.TMP_Text m_Text;

        [SerializeField]
        private Gradient2 m_Gradient;
        private Action<ActionSheetOptionViewController> m_OnItemClicked;
        private Button m_Button;

        private IAnimatedTransition m_Transition;

        private string message
        {
            set { m_Text.text = value; }
        }

        private string image
        {
            set
            {
                if(string.IsNullOrEmpty(value))
                    m_ImageHolder.SetActive(false);

                m_CachedImage.url = value;
            }
        }

        public ActionSheetOption option { get; private set; }

        private void Awake()
        {
            m_Button = GetComponent<Button>();
            m_Transition = GetComponent<IAnimatedTransition>();
            
            Assert.IsNotNull(m_CachedImage);
            Assert.IsNotNull(m_Text);
            Assert.IsNotNull(m_ImageHolder);
        }

        public void Initialize(ActionSheetOption option, Action<ActionSheetOptionViewController> onItemClicked)
        {
            this.option = option;
            this.image = option.icon;
            this.message = option.message;
            m_OnItemClicked = onItemClicked;
        }

        public async void DisableButton()
        {
            m_Button.enabled = false;
            await m_Transition?.PlayExitAsync();
        }

        public async void OnItemClicked()
        {
            m_OnItemClicked?.Invoke(this);
            m_Button.enabled = false;

            await m_Transition?.PlayEnterAsync();
        }
    }
}