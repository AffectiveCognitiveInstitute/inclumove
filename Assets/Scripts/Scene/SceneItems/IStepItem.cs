using System;
using System.Collections.Generic;
using Aci.Unity.Data;
using Unity.Mathematics;

namespace Aci.Unity.Scene.SceneItems
{
    public interface IStepItem : IDisposable
    {
        IReadOnlyList<ISceneItem> sceneItems { get; }
        IIdentifiable<Guid> identifiable { get; }
        float3 duration { get; set; }
        int repetitions { get; set; }
        bool automatic { get; set; }
        int triggerId { get; set; }
        string name { get; set; }
        bool Add(ISceneItem item);
        bool Remove(ISceneItem item);
    }

}
