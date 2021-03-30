using System;
using UnityEngine;

namespace Aci.Unity.Data
{
    public class UIntIdentifier : MonoBehaviour, IIdentifiable<uint>
    {
        private IIdProviderService<uint> m_IdProvider;

        [SerializeField]
        private uint m_Id = uint.MinValue;

        public uint identifier
        {
            get => m_Id;
            set
            {
                if (m_IdProvider.HasId(value) || value == uint.MinValue)
                    return;
                m_IdProvider.Unregister(this);
                m_Id = value;
                m_IdProvider.Register(this);
            }
        }

        [Zenject.Inject]
        private void Construct(IIdProviderService<uint> idProvider)
        {
            m_IdProvider = idProvider;

            uint newId = m_IdProvider.Register(this);
            if (newId != uint.MinValue)
                m_Id = newId;
        }

        void OnDestroy()
        {
            m_IdProvider.Unregister(this);
        }
    }
}
