using UnityEngine;
using Zenject;

namespace Aci.Unity.Scene
{
    [CreateAssetMenu(fileName = "SceneItemTemplateDataInstaller", menuName = "ScriptableObjects/Scene/SceneItemTemplateDataInstaller")]
    class SceneItemTemplateDataInstaller : ScriptableObjectInstaller
    {
        public SceneItemTemplateDataStorage dataStorage;

        public override void InstallBindings()
        {
            Container.BindInstances(dataStorage);
        }
    }
}
