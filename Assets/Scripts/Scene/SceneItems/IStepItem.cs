using System;
using System.Collections.Generic;
using Aci.Unity.Data;
using Unity.Mathematics;

namespace Aci.Unity.Scene.SceneItems
{
    public interface IStepItem : IDisposable
    {
        IReadOnlyList<ISceneItem> sceneItems { get; }
        IIdentifiable<uint> identifiable { get; }
        float3 duration { get; set; }
        int repetitions { get; set; }
        bool automatic { get; set; }
        bool mount { get; set; }
        bool unmount { get; set; }
        bool control { get; set; }
        int partId { get; set; }
        int gripperId { get; set; }
        string name { get; set; }
        bool Add(ISceneItem item);
        bool Remove(ISceneItem item);
    }

}
