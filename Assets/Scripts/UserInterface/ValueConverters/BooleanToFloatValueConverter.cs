using Aci.UI.Binding;
using UnityEngine;

namespace Aci.Unity.UserInterface.ValueConverters
{
    [CreateAssetMenu(menuName = "ACI/Value Converters/Boolean To Float Converter")]
    public class BooleanToFloatValueConverter : ScriptableObject, IValueConverter
    {
        [SerializeField]
        private float m_TrueValue;

        [SerializeField]
        private float m_FalseValue;

        public object Convert(object value)
        {
            bool b = (bool)value;
            return b ? m_TrueValue : m_FalseValue;
        }

        public object ConvertBack(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}

