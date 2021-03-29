using Aci.Unity.UI.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface.Animation
{
    [RequireComponent(typeof(Toggle), typeof(TweenerDirector))]
    public class ToggleTweenerDirectorBridge : MonoBehaviour
    {
        private TweenerDirector m_TweenerDirector;
        private Toggle m_Toggle;
        private bool m_IsOn;
        private void Awake()
        {
            m_Toggle = GetComponent<Toggle>();
            m_IsOn = m_Toggle.isOn;
            m_TweenerDirector = GetComponent<TweenerDirector>();
            m_TweenerDirector.Seek(m_IsOn ? 1 : 0);
        }

        private void OnEnable()
        {
            m_Toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            m_Toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool isOn)
        {
            if (isOn == m_IsOn)
                return;

            m_IsOn = isOn;
            if (isOn)
                m_TweenerDirector.PlayForwards();
            else
                m_TweenerDirector.PlayReverse();
        }
    }
}

