using Aci.UI.Binding;
using UnityEngine;

namespace Aci.Unity.UserInterface.ValueConverters
{
    [CreateAssetMenu(menuName = "ACI/Value Converters/Boolean To Sprite Converter")]
    public class BooleanToSpriteValueConverter : ScriptableObject, IValueConverter
    {
        [SerializeField]
        private Sprite m_TrueValue;

        [SerializeField]
        private Sprite m_FalseValue;

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

