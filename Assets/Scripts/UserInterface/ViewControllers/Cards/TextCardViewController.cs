using Aci.Unity.UI;
using UnityEngine;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class TextCardViewController : MonoBehaviour
    {
        [SerializeField]
        private ReplaceEmojiInText m_Text;

        protected string message
        {
            set { m_Text.text = value; }
        }

        public void Initialize(string message)
        {
            this.message = message;
        }
    }
}