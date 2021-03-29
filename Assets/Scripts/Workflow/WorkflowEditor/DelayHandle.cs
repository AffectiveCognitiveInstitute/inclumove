using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Aci.Unity.Scene;
using UnityEngine;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class DelayHandle : MonoBehaviour, ISubHandle
    {
        private ISceneItemRegistry m_ItemRegistry;
        private SelectionHandle m_ParentHandle;

        private bool m_IsActive = false;

        [SerializeField]
        private RectTransform m_SelectorTransform;

        [SerializeField]
        private TMPro.TMP_InputField m_Input;
        private IFormatProvider m_Format;

        void Awake()
        {
            m_Format = new CultureInfo("en");
            m_ParentHandle = transform.parent.GetComponent<SelectionHandle>();
            handleActive.AddListener(m_ParentHandle.OnHandleActivated);
        }

        [Zenject.Inject]
        private void Construct(ISceneItemRegistry registry)
        {
            m_ItemRegistry = registry;
        }

        public SelectionHandle.HandleActiveEvent handleActive { get; } = new SelectionHandle.HandleActiveEvent();

        public void Toggle()
        {
            ToggleActive(!m_IsActive, false);
        }

        public void ToggleActive(bool enabled, bool silent)
        {
            m_IsActive = enabled;
            if (m_IsActive)
                m_Input.text = m_ParentHandle.target.payloadViewController.delay.ToString(m_Format);
            m_SelectorTransform.gameObject.SetActive(enabled);
            if (silent)
                return;
            handleActive.Invoke(this, enabled, false);
        }

        public void ToggleHidden(bool hidden)
        {
            gameObject.SetActive(!hidden);
        }

        public void OnDelayEdited(string delay)
        {
            float newDelay = float.Parse(delay, m_Format);
            m_ParentHandle.target.payloadViewController.delay = newDelay;
        }
    }
}
