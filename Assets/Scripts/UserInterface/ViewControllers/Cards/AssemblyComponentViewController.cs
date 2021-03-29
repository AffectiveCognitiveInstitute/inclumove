using Aci.Unity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class AssemblyComponentViewController : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text m_TitleLabel;

        [SerializeField]
        private TMPro.TMP_Text m_LocationLabel;

        [SerializeField]
        private CachedImageComponent m_CachedImage;

        private string title
        {
            set { m_TitleLabel.text = value; }
        }

        private string icon
        {
            set
            {
                m_CachedImage.url = value;
            }
        }

        private string location
        {
            set { m_LocationLabel.text = value; }
        }

        public void Initialize(AssemblyComponent component)
        {
            title = $"{component.title} x{component.quantity}";
            location = component.location;
            icon = component.icon;
        }
    }
}