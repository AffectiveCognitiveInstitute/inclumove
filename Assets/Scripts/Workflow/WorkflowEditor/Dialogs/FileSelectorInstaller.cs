using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class FileSelectorInstaller : MonoInstaller<FileSelectorInstaller>
    {
        [SerializeField]
        private GameObject m_FileSelectorPrefab;

        [SerializeField]
        private Transform m_PopupLayer;

        public override void InstallBindings()
        {
            Container.BindFactory<FileSelectedDelegate, IEnumerable<string>, IEnumerable<string>, FileSelectorViewController, FileSelectorViewController.Factory>().
                      FromComponentInNewPrefab(m_FileSelectorPrefab).UnderTransform(m_PopupLayer);

            Container.Bind<FileSelectorFacade>().ToSelf().AsCached();
        }
    }
}