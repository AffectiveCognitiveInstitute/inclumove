using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aci.Unity.UserInterface.Style
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Style")]
    public class Style : ScriptableObject, IStyle
    {
        [SerializeField]
        private List<OverridableAttribute> m_Attributes = new List<OverridableAttribute>();

        /// <inheritdoc />
        public OverridableAttribute this[int index] => m_Attributes[index];

        public OverridableAttribute this[string name] => m_Attributes.FirstOrDefault(x => x.attributeName == name);

        /// <inheritdoc />
        public int count => m_Attributes.Count;

        /// <inheritdoc />
        public void AddAttribute(OverridableAttribute attribute)
        {
            if (m_Attributes.Any(x => x.attributeName == attribute.attributeName))
                throw new ArgumentException($"The attribute with the name: {attribute.attributeName} already exists.");

            m_Attributes.Add(attribute);
        }

        /// <inheritdoc />
        public T GetAttribute<T>(string attributeName) where T : OverridableAttribute
        {
            OverridableAttribute attribute = m_Attributes.FirstOrDefault(x => x.attributeName == attributeName);

            if(attribute == null)
                throw new ArgumentException($"The attribute with the name: {attribute.attributeName} already exists.");

            return attribute as T;
        }

        public IEnumerator<OverridableAttribute> GetEnumerator()
        {
            foreach (OverridableAttribute a in m_Attributes)
                yield return a;
        }

        /// <inheritdoc />
        public bool RemoveAttribute(OverridableAttribute attribute)
        {
            return m_Attributes.Remove(attribute);
        }

        /// <inheritdoc />
        public void SetAttributeValue<T>(string attributeName, T attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentNullException(nameof(attributeName));

            OverridableAttribute a = m_Attributes.FirstOrDefault(x => x.attributeName == attributeName);

            if (a == null)
                throw new Exception($"Could not find Attribute named {attributeName}.");

            a.Set(attributeValue);
        }

        /// <inheritdoc />
        public bool TryGetAttribute<T>(string attributeName, out T attribute) where T : OverridableAttribute
        {
            attribute = null;
            OverridableAttribute a = m_Attributes.FirstOrDefault(x => x.attributeName == attributeName);

            if(a != null)
            {
                attribute = a as T;
                return true;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

