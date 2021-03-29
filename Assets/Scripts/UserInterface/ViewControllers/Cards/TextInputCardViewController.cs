using System;
using UnityEngine;

namespace Aci.Unity.UserInterface.ViewControllers
{
    public class TextInputCardViewController : TextCardViewController
    {
        [SerializeField]
        private TMPro.TMP_InputField m_InputField;

        [SerializeField]
        private TMPro.TMP_Text m_TitleLabel;

        private string m_Input;

        private Action<string> m_OnInputSubmitted;

        private bool isPassword
        {
            set { m_InputField.contentType = value ? TMPro.TMP_InputField.ContentType.Password : 
                                                     TMPro.TMP_InputField.ContentType.Standard; }
        }

        private string title
        {
            set { m_TitleLabel.text = value; }
        }

        public void Initialize(string message, string title, Action<string> onInputSubmitted, bool isPassword = false)
        {
            this.message = message;
            m_OnInputSubmitted = onInputSubmitted;
            this.isPassword = isPassword;
        }

        public void OnInputChanged(string value)
        {
            m_Input = value;
        }

        public void OnInputSubmitted()
        {
            m_OnInputSubmitted?.Invoke(m_Input);
        }
    }
}