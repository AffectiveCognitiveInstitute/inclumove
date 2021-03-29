using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BotConnector.Unity.UI
{
    public static class Helper
    {
        public static float SingleLineHeight = EditorGUIUtility.singleLineHeight + 2;

        public static ReorderableList GetListWithFoldout(SerializedObject serializedObject, SerializedProperty property, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
        {
            var list = new ReorderableList(serializedObject, property, draggable, displayHeader, displayAddButton, displayRemoveButton);

            list.drawHeaderCallback = (Rect rect) => {
                var newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
                property.isExpanded = EditorGUI.Foldout(newRect, property.isExpanded, property.displayName, true);
            };

            return list;
        }
    }
}
