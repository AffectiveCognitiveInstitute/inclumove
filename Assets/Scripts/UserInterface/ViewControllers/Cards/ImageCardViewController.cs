using System;
using Aci.Unity.UI;
using Aci.Unity.UserInterface.Factories;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class ImageCardViewController : TextCardViewController
    {
        [SerializeField]
        private CachedImageComponent m_CachedImage;
        [SerializeField]
        private AspectRatioFitter m_AspectRatioFitter;

        private string m_ImageUrl;

        protected string imageUrl
        {
            get { return m_ImageUrl; }
            set
            {
                m_ImageUrl = value;
                m_CachedImage.url = value;
            }
        }

        private void Awake()
        {
            m_CachedImage.spriteLoaded += OnSpriteLoaded;
        }

        private void OnSpriteLoaded(Sprite s)
        {
            m_AspectRatioFitter.aspectRatio = s.rect.width / s.rect.height;
        }
    }

    public struct AssemblyComponent
    {
        public string icon { get; }
        public string title { get; }
        public int quantity { get; }
        public string location { get; }

        public AssemblyComponent(string icon, string title, string location, int quantity)
        {
            this.icon = icon;
            this.title = title;
            this.quantity = quantity;
            this.location = location;
        }
    }
}