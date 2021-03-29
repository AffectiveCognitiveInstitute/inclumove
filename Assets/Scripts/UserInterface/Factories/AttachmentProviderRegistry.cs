using System;
using System.Collections.Generic;

namespace Aci.Unity.UserInterface.Factories
{
    public class AttachmentProviderRegistry : IAttachmentProviderRegistry
    {
        private readonly Dictionary<string, IAttachmentProvider> m_AttachmentHandlers = new Dictionary<string, IAttachmentProvider>(5);

        /// <inheritdoc />
        public void Register(IAttachmentProvider handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (string.IsNullOrEmpty(handler.type))
                throw new ArgumentNullException(nameof(handler.type));

            m_AttachmentHandlers.Add(handler.type, handler);
        }

        /// <inheritdoc />
        public bool TryGetProvider(string contentType, out IAttachmentProvider provider)
        {
            return m_AttachmentHandlers.TryGetValue(contentType, out provider);
        }

        /// <inheritdoc />
        public void Unregister(IAttachmentProvider handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (string.IsNullOrEmpty(handler.type))
                throw new ArgumentNullException(nameof(handler.type));

            m_AttachmentHandlers.Remove(handler.type);
        }
    }
}