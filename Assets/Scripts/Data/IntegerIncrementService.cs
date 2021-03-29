using System.Collections.Generic;
using System.Linq;

namespace Aci.Unity.Data
{
    internal class IntegerIncrementService : IIdProviderService<int>
    {
        private List<IIdentifiable<int>> m_IdReceivers = new List<IIdentifiable<int>>();

        private int GetFirstID()
        {
            int count = 0;
            while (m_IdReceivers.Any(x => x.identifier == count))
                ++count;
            return count;
        }

        /// <inheritdoc />
        public int Register(IIdentifiable<int> identifiable)
        {
            if(m_IdReceivers.Contains(identifiable))
                return -1;
            int selectedId = identifiable.identifier == -1 ? GetFirstID() : identifiable.identifier;
            m_IdReceivers.Add(identifiable);
            return selectedId;
        }

        /// <inheritdoc />
        public void Unregister(IIdentifiable<int> identifiable)
        {
            m_IdReceivers.Remove(identifiable);
        }

        public bool HasId(int id) => m_IdReceivers.Any(x => x.identifier == id);
    }
}
