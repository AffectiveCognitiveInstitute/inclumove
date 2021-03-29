using Aci.Unity.Events;
using Aci.Unity.Scene.SceneItems;

namespace Aci.Unity.Scene
{
    class SelectionService : ISelectionService
    {
        private IAciEventManager m_EventBroker;

        public ISelectable currentSelection { get; private set; }

        [Zenject.Inject]
        private void Construct(IAciEventManager broker)
        {
            m_EventBroker = broker;
        }

        public void Register(ISelectable selectable)
        {
            selectable.selectionChanged.AddListener(OnSelectionChanged);
        }


        public void Unregister(ISelectable selectable)
        {
            selectable.selectionChanged.RemoveListener(OnSelectionChanged);
        }

        public void Select(ISelectable selectable)
        {
            if (selectable != null)
            {
                selectable.isSelected = true;
                return;
            }
            if (currentSelection != null)
            {
                currentSelection.isSelected = false;
            }
        }

        private void OnSelectionChanged(ISelectable selectable)
        {
            SelectionChanged args = new SelectionChanged();
            if (selectable.isSelected)
            {
                args.previous = currentSelection;
                args.current = selectable;
                currentSelection = selectable;
                if(args.previous != null)
                    args.previous.isSelected =false;
            }
            else if(selectable == currentSelection)
            {
                args.previous = currentSelection;
                args.current = null;
                currentSelection = null;
            }
            else
            {
                return;
            }
            m_EventBroker.Invoke(args);
        }
    }
}
