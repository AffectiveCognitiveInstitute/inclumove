using System;
using System.Collections.Generic;
using System.Linq;

namespace Aci.Unity.Data
{
    internal class GuidService : IIdProviderService<Guid>
    {
        private List<IIdentifiable<Guid>> m_IdReceivers = new List<IIdentifiable<Guid>>();

        private Guid GetGuid()
        {
            Guid newGuid = Guid.NewGuid();
            while (m_IdReceivers.Any(x => x.identifier == newGuid))
                newGuid = Guid.NewGuid();
            return newGuid;
        }

        /// <inheritdoc />
        public Guid Register(IIdentifiable<Guid> identifiable)
        {
            if(m_IdReceivers.Contains(identifiable))
                return Guid.Empty;
            Guid selectedId = identifiable.identifier.Equals(Guid.Empty) ? GetGuid() : identifiable.identifier;
            m_IdReceivers.Add(identifiable);
            return selectedId;
        }

        /// <inheritdoc />
        public void Unregister(IIdentifiable<Guid> identifiable)
        {
            m_IdReceivers.Remove(identifiable);
        }

        public bool HasId(Guid id)
        {
            bool result = m_IdReceivers.Any(x => x.identifier == id);
            return result;
        }
    }
}
