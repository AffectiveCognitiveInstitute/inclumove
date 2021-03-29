using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aci.Unity.Scene;
using Aci.Unity.Scene.SceneItems;
using TMPro;
using UnityEngine;

namespace Aci.Unity.Scene
{
    [CreateAssetMenu(fileName = "SceneItemTemplateData", menuName = "ScriptableObjects/Scene/SceneItemTemplateData")]
    public class SceneItemTemplateData : ScriptableObject
    {
        public PayloadType payloadType;
        public string itemName;
        public Sprite placeholderIcon;
        public GameObject sceneItemPrefab;
    }
}
