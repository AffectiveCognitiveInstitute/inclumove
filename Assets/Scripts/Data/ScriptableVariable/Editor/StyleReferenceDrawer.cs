
namespace Aci.Unity.Data
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(StyleReference))]
    public class StyleReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            VariableReferenceDrawer.OnGUI(position, property, label);
        }
    }
}
