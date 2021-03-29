using Aci.Unity.Scene.SceneItems;

namespace Aci.Unity.Scene
{
    interface ISelectionService
    {
        ISelectable currentSelection { get; }

        void Register(ISelectable selectable);

        void Unregister(ISelectable selectable);

        void Select(ISelectable selectable);
    }
}
