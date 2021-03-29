using System;
using UnityEngine;

namespace Aci.Unity.Data
{
    public class GuidIdentifier : MonoBehaviour, IIdentifiable<Guid>
    {
        private IIdProviderService<Guid> m_IdProvider;

        [SerializeField]
        private Guid m_Id = Guid.Empty;

        public Guid identifier
        {
            get => m_Id;
            set
            {
                if (m_IdProvider.HasId(value) || value == Guid.Empty)
                    return;
                m_IdProvider.Unregister(this);
                m_Id = value;
                m_IdProvider.Register(this);
            }
        }

        [Zenject.Inject]
        private void Construct(IIdProviderService<Guid> idProvider)
        {
            m_IdProvider = idProvider;

            Guid newId = m_IdProvider.Register(this);
            if (newId != Guid.Empty)
                m_Id = newId;
        }

        void OnDestroy()
        {
            m_IdProvider.Unregister(this);
        }
    }
}
