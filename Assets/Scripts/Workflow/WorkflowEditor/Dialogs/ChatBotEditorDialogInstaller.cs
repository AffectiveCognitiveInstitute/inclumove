using Aci.Unity.Data.JsonModel;
using System;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class ChatBotEditorDialogInstaller : MonoInstaller<ChatBotEditorDialogInstaller>
    {
        [SerializeField]
        private GameObject m_Prefab;
        [SerializeField]
        private Transform m_PopupLayer;

        public override void InstallBindings()
        {
            Container.BindFactory<ActivityData, Action<ActivityData>, ChatBotEditorDialogViewController, ChatBotEditorDialogViewController.Factory>().
                      FromComponentInNewPrefab(m_Prefab).
                      UnderTransform(m_PopupLayer);
        }
    }
}