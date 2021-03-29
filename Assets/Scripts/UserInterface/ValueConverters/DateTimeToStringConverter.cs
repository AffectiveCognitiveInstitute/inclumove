using Aci.UI.Binding;
using System;
using System.Globalization;
using UnityEngine;

namespace Aci.Unity.UserInterface.ValueConverters
{
    [CreateAssetMenu(menuName = "ACI/Value Converters/DateTime To Day String Converter")]
    public class DateTimeToStringConverter : ScriptableObject, IValueConverter
    {
        [SerializeField]
        private string m_Format;

        public object Convert(object value)
        {
            DateTime dateTime = (DateTime)value;
            return dateTime.ToString(@m_Format, CultureInfo.GetCultureInfo("de-DE"));
        }

        public object ConvertBack(object value)
        {
            throw new NotImplementedException();
        }
    }
}
