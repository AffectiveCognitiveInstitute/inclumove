using System.Collections.Generic;
using Aci.Unity.Scene.SceneItems;
using Zenject;

namespace Aci.Unity.Data.JsonModel
{
    class JsonModelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindFactory<ISceneItem, SceneItemData, SceneItemData.Factory>().FromFactory<SceneItemDataFactory>();
            Container.BindFactory<IStepItem, WorkflowStepData, WorkflowStepData.Factory>().FromFactory<WorkflowStepDataFactory>();
            Container.BindFactory<List<IStepItem>, WorkflowData, WorkflowData.Factory>().FromFactory<WorkflowDataFactory>();
        }
    }
}
