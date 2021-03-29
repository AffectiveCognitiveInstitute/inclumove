using UnityEditor;
using UnityEngine;

namespace BotConnector.Unity.UI
{
    [CustomPropertyDrawer(typeof(AttachmentHandler))]
    class AttachmentHandlerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            var type = property.FindPropertyRelative("ContentType");
            var template = property.FindPropertyRelative("Template");

            var width = position.width;
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            position.width = (width * 0.6f) - 5;

            EditorGUI.PropertyField(position, type, GUIContent.none);

            position.x += position.width + 5;
            position.width = (width * 0.4f);

            EditorGUI.PropertyField(position, template, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
