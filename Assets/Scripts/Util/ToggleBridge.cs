using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Aci.Unity.Util
{
    public class ToggleBridge : MonoBehaviour
    {
        [SerializeField]
        private bool m_CurrentState = false;

        [SerializeField]
        private UnityEvent OnActivated;

        [SerializeField]
        private UnityEvent OnDeactivated;

        public void Toggle()
        {
            if (m_CurrentState)
                OnActivated.Invoke();
            else
                OnDeactivated.Invoke();
            m_CurrentState = !m_CurrentState;
        }
    }
}
