using UnityEngine;

namespace Aci.UI.Binding
{
    [CreateAssetMenu(menuName = "ACI/Value Converters/Boolean To Gradient Converter")]
    public class BooleanToGradientConverter : ScriptableObject, IValueConverter
    {
        [SerializeField]
        private Gradient m_TrueValue;

        [SerializeField]
        private Gradient m_FalseValue;

        public object Convert(object value)
        {
            bool bValue = (bool)value;
            return bValue ? m_TrueValue : m_FalseValue;
        }

        public object ConvertBack(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}