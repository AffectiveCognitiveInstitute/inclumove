using Microsoft.Bot.Connector.DirectLine;
using UnityEngine;

namespace Aci.Unity.UserInterface.Factories
{
    public abstract class AttachmentProviderBase : MonoBehaviour, IAttachmentProvider
    {
        private IAttachmentProviderRegistry m_Registry;

        /// <inheritdoc/>
        public abstract string type { get; }
        
        [Zenject.Inject]
        private void Create(IAttachmentProviderRegistry registry)
        {
            m_Registry = registry;
        }

        protected virtual void OnEnable()
        {
            m_Registry.Register(this);
        }

        protected virtual void OnDisable()
        {
            m_Registry.Unregister(this);
        }

        /// <inheritdoc/>
        public abstract GameObject GetGameObject(Activity activity);
    }
}
