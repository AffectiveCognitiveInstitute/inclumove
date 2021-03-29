using Aci.Unity.Data.JsonModel;
using Aci.Unity.Scene.SceneItems;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Scene
{
    class ItemSceneInstaller : MonoInstaller
    {
        public Transform sceneItemRoot;

        [SerializeField]
        private ItemMode m_ItemMode = ItemMode.Workflow;

        public override void InstallBindings()
        {
            Container.Bind<Transform>().WithId("SceneItemRoot").FromInstance(sceneItemRoot).AsCached();
            Container.Bind<ISceneItemRegistry>().To<SceneItemRegistry>().AsSingle();
            Container.BindFactory<SceneItemData, ISceneItem, SceneItem.Factory>().FromFactory<SceneItemFactory>();
            Container.BindFactory<WorkflowStepData, UnityEngine.Object, IStepItem, StepItem.Factory>().FromFactory<StepItemFactory>();
            Container.Bind<ItemMode>().FromInstance(m_ItemMode).AsSingle();
        }
    }
}
