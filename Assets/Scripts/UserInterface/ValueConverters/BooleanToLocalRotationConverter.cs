using UnityEngine;

namespace Aci.UI.Binding
{
    [CreateAssetMenu(menuName = "ACI/Value Converters/Boolean To Local Rotation Converter")]
    public class BooleanToLocalRotationConverter : ScriptableObject, IValueConverter
    {
        [SerializeField]
        private Vector3 m_TrueValue;

        [SerializeField]
        private Vector3 m_FalseValue;

        public object Convert(object value)
        {
            bool bValue = (bool)value;
            return Quaternion.Euler(bValue ? m_TrueValue : m_FalseValue);
        }

        public object ConvertBack(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}