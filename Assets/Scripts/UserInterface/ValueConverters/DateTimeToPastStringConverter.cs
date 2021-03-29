using Aci.UI.Binding;
using System;
using UnityEngine;

namespace Aci.Unity.UserInterface.ValueConverters
{
    [CreateAssetMenu(menuName = "ACI/Value Converters/DateTime To Past String Converter")]
    public class DateTimeToPastStringConverter : ScriptableObject, IValueConverter
    {
        public object Convert(object value)
        {
            if (!(value is DateTime))
                throw new ArgumentException($"Expected {nameof(value)} to be of type: {typeof(DateTime)}", nameof(value));

            DateTime now = DateTime.UtcNow;
            DateTime then = (DateTime)value;
            TimeSpan timeSpan = now - then;

            if(timeSpan.TotalDays >= 365)
            {
                // Years
                int years = Mathf.FloorToInt((float)timeSpan.TotalDays / 365);
                if (years > 1)
                    return $"Vor {years} Jahren";

                return "Vor einem Jahr";
            }
            else if(timeSpan.TotalDays >= 30)
            {
                // Month
                int months = Mathf.FloorToInt((float)timeSpan.TotalDays / 30);
                if (months > 1)
                    return $"Vor {months} Monaten";

                return "Vor einem Monat";
            }
            else if(timeSpan.TotalDays >= 7)
            {
                // Weeks
                int weeks = Mathf.FloorToInt((float) timeSpan.TotalDays / 7);

                if (weeks > 1)
                    return $"Vor {weeks} Wochen";

                return "Vor einer Woche";
            }
            else if(now.Day - then.Day >= 1)
            {
                // Days
                int days = now.Day - then.Day;

                if (days > 1)
                    return $"Vor {days} Tagen";

                return "Gestern";
            }

            return "Heute";            
        }

        public object ConvertBack(object value)
        {
            throw new NotImplementedException();
        }
    }
}
