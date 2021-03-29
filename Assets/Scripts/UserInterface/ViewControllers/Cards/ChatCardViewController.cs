using Aci.Unity.UserInterface.Animation;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class ChatCardViewController : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_AvatarContainer;

        [SerializeField]
        private GameObject m_AvatarDecorator;

        [SerializeField]
        private Image m_AvatarImage;

        [SerializeField]
        private Transform m_ContentContainer;

        [SerializeField]
        private bool m_isHighlighted;

        [SerializeField]
        private Color m_CardColor;

        [SerializeField]
        private Color m_HighlightColor;

        public Color cardColor
        {
            get => m_CardColor;
            set => m_CardColor = value;
        }

        public Color highlightColor
        {
            get => m_HighlightColor;
            set => m_HighlightColor = value;
        }

        private IAnimatedTransition m_Transition;

        private void Awake()
        {
            m_Transition = GetComponent<IAnimatedTransition>();

            Assert.IsNotNull(m_AvatarContainer);

            if (m_Transition != null)
                m_Transition.PlayEnterAsync();
        }

        public async void SetAvatarImage(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
                throw new ArgumentNullException(nameof(resourcePath));

            if(m_AvatarImage != null)
                m_AvatarImage.sprite = await Resources.LoadAsync(resourcePath) as Sprite;
        }

        public void SetAvatarImage(Sprite sprite)
        {
            if (sprite == null)
                throw new ArgumentNullException(nameof(sprite));

            if (m_AvatarImage != null)
                m_AvatarImage.sprite = sprite;
        }

        public void SetAvatarEnabled(bool isEnabled)
        {
            if (m_AvatarContainer != null)
                m_AvatarContainer.SetActive(isEnabled);
            if(m_AvatarDecorator != null)
                m_AvatarDecorator.SetActive(isEnabled);
        }

        public void SetHighlighted(bool isHighlighted)
        {
            if (m_ContentContainer == null || m_isHighlighted == isHighlighted)
                return;
            m_isHighlighted = isHighlighted;
            for (int i = 0; i < m_ContentContainer.childCount; ++i)
            {
                Image img = m_ContentContainer.GetChild(i).GetComponent<Image>();
                if (img == null) continue;
                img.color = isHighlighted ? m_HighlightColor : m_CardColor;
            }
        }

        public void AddContent(GameObject content)
        {
            if(content == null)
                throw new ArgumentNullException(nameof(content));

            content.transform.SetParent(m_ContentContainer, false);
            content.GetComponent<Image>().color = m_isHighlighted ? m_HighlightColor : m_CardColor;
        }

        public class Factory : PlaceholderFactory<UnityEngine.Object, ChatCardViewController>
        {
        }
    }
}
