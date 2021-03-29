using System.Collections.Generic;
using System.Linq;
using Aci.Unity.Scene.SceneItems;
using UnityEngine;

namespace Aci.Unity.Scene
{
    [CreateAssetMenu(fileName = "SceneItemTemplateDataStorage", menuName = "ScriptableObjects/Scene/SceneItemTemplateDataStorage")]
    class SceneItemTemplateDataStorage : ScriptableObject
    {
        public List<SceneItemTemplateData> data = new List<SceneItemTemplateData>();

        public SceneItemTemplateData GetDataForType(PayloadType type, string name = "")
        {
            return data.Find(x => x.payloadType == type && (x.payloadType != PayloadType.Primitive || x.itemName == name));
        }
    }
}
