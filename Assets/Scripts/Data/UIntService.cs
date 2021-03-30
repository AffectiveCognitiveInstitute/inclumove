using System;
using System.Collections.Generic;
using System.Linq;

namespace Aci.Unity.Data
{
    internal class UIntService : IIdProviderService<uint>
    {
        private List<IIdentifiable<uint>> m_IdReceivers = new List<IIdentifiable<uint>>();

        System.Random m_Rand = new System.Random();
        private uint GetUInt()
        {
            uint newUInt = 0;
            unchecked
            {
                newUInt = (uint)m_Rand.Next();
            };
            while (m_IdReceivers.Any(x => x.identifier == newUInt))
            {
                newUInt = (uint)m_Rand.Next();
            }
                
            return newUInt;
        }

        /// <inheritdoc />
        public uint Register(IIdentifiable<uint> identifiable)
        {
            if (m_IdReceivers.Contains(identifiable))
                return uint.MinValue;
            uint selectedId = identifiable.identifier.Equals(uint.MinValue) ? GetUInt() : identifiable.identifier;
            m_IdReceivers.Add(identifiable);
            return selectedId;
        }

        /// <inheritdoc />
        public void Unregister(IIdentifiable<uint> identifiable)
        {
            m_IdReceivers.Remove(identifiable);
        }

        public bool HasId(uint id)
        {
            bool result = m_IdReceivers.Any(x => x.identifier == id);
            return result;
        }
    }
}
