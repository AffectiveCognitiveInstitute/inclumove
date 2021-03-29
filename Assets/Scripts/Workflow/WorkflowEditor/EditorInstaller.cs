using Aci.Unity.Scene;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    class EditorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<WorkflowEditorSceneManager>().FromComponentInHierarchy().AsSingle();
            Container.BindFactory<SceneItemTemplateData, string, UnityEngine.Object, SceneItemElementViewController,
                    SceneItemElementViewController.Factory>().FromFactory<SceneItemElementViewControllerFactory>();
        }
    }
}
