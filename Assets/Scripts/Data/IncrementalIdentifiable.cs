using UnityEngine;

namespace Aci.Unity.Data
{
    public class IncrementalIdentifiable : MonoBehaviour, IIdentifiable<int>
    {
        private IIdProviderService<int> m_IdProvider;

        [SerializeField]
        private int m_Id = -1;

        public int identifier
        {
            get => m_Id;
            set
            {
                if (m_IdProvider.HasId(value))
                    return;
                m_IdProvider.Unregister(this);
                m_Id = value;
                m_IdProvider.Register(this);
            }
        }

        [Zenject.Inject]
        private void Construct(IIdProviderService<int> idProvider)
        {
            m_IdProvider = idProvider;

            int newId = m_IdProvider.Register(this);
            if (newId != -1)
                m_Id = newId;
        }

        void OnDestroy()
        {
            m_IdProvider?.Unregister(this);
        }
    }
}
