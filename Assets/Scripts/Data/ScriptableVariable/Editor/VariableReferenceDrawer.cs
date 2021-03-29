using UnityEditor;
using UnityEngine;

namespace Aci.Unity.Data
{
    public class VariableReferenceDrawer
    {
        private static GUIStyle s_Style = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));

        public static void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect p = position;
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty useConstantProperty = property.FindPropertyRelative("m_UseConstant");
            SerializedProperty constantProperty = property.FindPropertyRelative("m_Constant");
            SerializedProperty variableProperty = property.FindPropertyRelative("m_Variable");

            EditorGUI.PrefixLabel(position, label);
            Rect dropdownRect = new Rect(p.x + EditorGUIUtility.labelWidth, p.y+4, 18, p.height);
            if (GUI.Button(dropdownRect, GUIContent.none, s_Style))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Use Constant"), useConstantProperty.boolValue, OnConstantPropertyClicked, useConstantProperty);
                menu.AddItem(new GUIContent("Use Scriptable Variable"), !useConstantProperty.boolValue, OnScriptableVariableClicked, useConstantProperty);
                menu.DropDown(new Rect(Event.current.mousePosition, position.size));
            }

            Rect fieldRect = new Rect(dropdownRect.x + dropdownRect.width, p.y,
                                      p.width - dropdownRect.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(fieldRect, useConstantProperty.boolValue ? constantProperty : variableProperty, GUIContent.none);


            EditorGUI.EndProperty();
        }

        private static void OnConstantPropertyClicked(object property)
        {
            (property as SerializedProperty).boolValue = true;
            (property as SerializedProperty).serializedObject.ApplyModifiedProperties();
        }

        private static void OnScriptableVariableClicked(object property)
        {
            (property as SerializedProperty).boolValue = false;
            (property as SerializedProperty).serializedObject.ApplyModifiedProperties();
        }
    }
}
