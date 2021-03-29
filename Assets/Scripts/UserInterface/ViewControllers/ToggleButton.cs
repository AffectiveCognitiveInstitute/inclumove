using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface.ViewControllers
{
    [RequireComponent(typeof(Button), typeof(Image)), ExecuteInEditMode]
    public class ToggleButton : MonoBehaviour
    {
        [System.Serializable]
        public class ToggledEvent : UnityEvent<bool> { }

        [SerializeField]
        private bool m_IsOn;

        [SerializeField]
        private Sprite m_OnSprite;

        [SerializeField]
        private Sprite m_OffSprite;
        
        public ToggledEvent onToggle;
        private Button m_Button;
        private Image m_Image;

        private Button button
        {
            get
            {
                if (m_Button == null)
                    m_Button = GetComponent<Button>();

                return m_Button;
            }
        }

        public bool isOn
        {
            get => m_IsOn;
            set
            {
                if (m_IsOn == value)
                    return;

                UpdateSprite(value);
                m_IsOn = value;
                onToggle?.Invoke(value);
            }
        }

        public bool interactable
        {
            get => button.interactable;
            set => button.interactable = value;
        }

        private void Awake()
        {
            m_Button = GetComponent<Button>();
            m_Image = GetComponent<Image>();
            UpdateSprite(m_IsOn);
        }

        private void OnEnable()
        {
            if(Application.isPlaying)
                button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            if(Application.isPlaying)
                button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            isOn = !isOn;
        }

        private void UpdateSprite(bool value)
        {
            try
            {
                if(m_Image != null)
                    m_Image.sprite = value ? m_OnSprite : m_OffSprite;
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateSprite(m_IsOn);
        }
#endif
    }
}