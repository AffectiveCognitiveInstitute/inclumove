using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class NewFileDialogInstaller : MonoInstaller<FileSelectorInstaller>
    {
        [SerializeField]
        private GameObject m_NewFileDialogPrefab;

        [SerializeField]
        private Transform m_PopupLayer;

        public override void InstallBindings()
        {
            Container.BindFactory<FileSelectedDelegate, string, string, NewFileDialogViewController, NewFileDialogViewController.Factory>().
                      FromComponentInNewPrefab(m_NewFileDialogPrefab).UnderTransform(m_PopupLayer);

            Container.Bind<NewFileDialogFacade>().ToSelf().AsCached();
        }
    }
}