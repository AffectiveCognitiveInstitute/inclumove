using System;
using UnityEngine;

namespace Aci.Unity.Data
{
    public abstract class ScriptableVariable<T> : ScriptableObject
    {
        [SerializeField]
        private bool m_SerializeValue = true;

        [SerializeField] private T m_SerializedValue;

        [NonSerialized] private T m_Value;

        public T value
        {
            get { return m_SerializeValue? m_SerializedValue : m_Value; }
            set
            {
                if(m_SerializeValue)
                {
                    m_SerializedValue = value;
                }
                else
                {
                    m_Value = value;
                }
            }
        }

        public static implicit operator T(ScriptableVariable<T> variable)
        {
            return variable.value;
        }

        protected virtual void OnEnable()
        {
            m_Value = default;
        }
    }
}

