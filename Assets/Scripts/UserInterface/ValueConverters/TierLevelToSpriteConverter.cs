using Aci.UI.Binding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aci.Unity.UserInterface.ValueConverters
{
    [CreateAssetMenu(menuName = "ACI/Value Converters/Tier Level To Sprite Converter")]
    public class TierLevelToSpriteConverter : ScriptableObject, IValueConverter
    {
        public Sprite[] tierSprites;

        public object Convert(object value)
        {
            int tierLevel = (int)value;

            if (tierLevel < 1 || tierLevel > 3)
                return null;

            return tierSprites[tierLevel - 1];
        }

        public object ConvertBack(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}

