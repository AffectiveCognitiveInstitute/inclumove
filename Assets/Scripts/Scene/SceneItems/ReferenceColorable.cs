using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.Scene.SceneItems
{
    class ReferenceColorable : MonoBehaviour, IColorable
    {
        [SerializeField]
        private SpriteRenderer m_ColoredImage;
        public Color color
        {
            get => m_ColoredImage.color;
            set => m_ColoredImage.color = value;
        }
    }
}
