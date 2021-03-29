using System;
using Aci.Unity.Scene;
using Aci.Unity.Scene.SceneItems;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Data.JsonModel
{
    [Serializable]
    public struct SceneItemData
    {
        public class Factory : PlaceholderFactory<ISceneItem, SceneItemData> { }

        public static SceneItemData Empty = new SceneItemData()
        {
            id = -1,
            levels = 0,
            position = Vector3.zero,
            size = Vector3.one,
            rotation = Vector3.zero,
            color = Color.white,
            payloadType = PayloadType.Primitive,
            payload = null,
            delay = 0f
        };

        public int id;
        public byte levels;
        public Vector3 position;
        public Vector3 size;
        public Vector3 rotation;
        public Color color;
        public PayloadType payloadType;
        public string payload;
        public float delay;
    }
}
