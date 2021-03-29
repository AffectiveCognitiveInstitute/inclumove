using UnityEngine;

namespace Aci.Unity.Gamification
{
    [CreateAssetMenu(menuName = "Inclumove/Color Scheme")]
    public class ColorScheme : ScriptableObject
    {
        [SerializeField]
        private Color m_PrimaryColor;
        [SerializeField]
        private Color m_SecondaryColor;

        public Color primaryColor => m_PrimaryColor;
        public Color secondaryColor => m_SecondaryColor;
    }
}