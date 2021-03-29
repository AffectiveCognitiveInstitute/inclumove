using Aci.Unity.UserInterface.Style;
using UnityEngine;

namespace Aci.Unity.Data
{
    [System.Serializable]
    public class StyleReference : VariableReferenceBase
    {
        [SerializeField]
        private Style m_Constant;

        [SerializeField]
        private StyleVariable m_Variable;

        public Style value
        {
            get { return m_UseConstant ? m_Constant : 
                                         m_Variable != null? m_Variable.value : null; }
        }

        public static implicit operator Style(StyleReference reference)
        {
            if (reference.value == null)
                return null;

            return reference.value;
        }
    }
}

