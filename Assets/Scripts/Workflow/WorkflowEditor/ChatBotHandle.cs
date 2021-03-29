using System;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Scene.SceneItems;
using Aci.Unity.UI.Dialog;
using UnityEngine;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class ChatBotHandle : MonoBehaviour, ISubHandle
    {
        private IDialogService m_DialogService;
        private ChatBotEditorDialogViewController.Factory m_DialogFactory;
        private SelectionHandle m_ParentHandle;

        public SelectionHandle.HandleActiveEvent handleActive { get; } = new SelectionHandle.HandleActiveEvent();

        [Zenject.Inject]
        private void Construct(IDialogService dialogService, ChatBotEditorDialogViewController.Factory dialogFactory)
        {
            m_DialogService = dialogService;
            m_DialogFactory = dialogFactory;
        }

        private void Awake()
        {
            m_ParentHandle = GetComponentInParent<SelectionHandle>();
            handleActive.AddListener(m_ParentHandle.OnHandleActivated);
        }

        public void OnHandleButtonClicked()
        {
            ToggleActive(true, false);
            ActivityData payload = ActivityData.Empty;
            try
            {
                if(!string.IsNullOrWhiteSpace(m_ParentHandle.target.payloadViewController.payload))
                    payload = JsonUtility.FromJson<ActivityData>(m_ParentHandle.target.payloadViewController.payload);
            }
            catch(ArgumentException)
            {
                // Invalid json
                Debug.LogWarning("Found invalid json.");
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }

            DialogComponent dialog = m_DialogFactory.Create(payload, OnActivityPayloadChanged);
            m_DialogService.SendRequest(DialogRequest.Create(dialog));
        }

        private void OnActivityPayloadChanged(ActivityData payload)
        {
            string json = JsonUtility.ToJson(payload, true);
            IPayloadViewController payloadViewController = m_ParentHandle.target.payloadViewController;
            payloadViewController.SetPayload(PayloadType.ChatBot, json, payloadViewController.delay);
        }

        public void ToggleActive(bool enabled, bool silent)
        {
            if (silent)
                return;
            handleActive?.Invoke(this, enabled, false);
        }

        public void ToggleHidden(bool hidden)
        {
            gameObject.SetActive(!hidden);
        }
    }
}
