using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aci.Unity.Data;
using UnityEngine;

namespace Aci.Unity.Scene.SceneItems
{
    public enum PayloadType
    {
        Primitive,
        Image,
        Video,
        Audio,
        Text,
        ChatBot
    }

    public enum ItemMode
    {
        Editor,
        Workflow
    }

    public interface ISceneItem
    {
        IIdentifiable<int> identifiable { get; }
        Transform itemTransform { get; }
        IPayloadViewController payloadViewController { get; }
        IColorable colorable { get; }
        ILevelable levelable { get; }
        IScalable scalable { get; }
    }
}
