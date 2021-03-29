using Aci.Unity.Scene.SceneItems;
using UnityEngine.Events;

namespace Aci.Unity.Scene
{
    public interface ISceneItemRegistry
    {
        UnityEvent<ISceneItem> itemRemoved { get; }
        UnityEvent<ISceneItem> itemAdded { get; }
        void RegisterItem(ISceneItem item);
        ISceneItem GetItemById(int id);
        void RemoveItemById(int id);
        void ClearRegistry();
    }
}
