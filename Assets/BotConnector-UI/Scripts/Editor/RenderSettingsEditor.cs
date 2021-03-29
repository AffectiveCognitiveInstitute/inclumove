using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BotConnector.Unity.UI
{
    [CustomEditor(typeof(RenderSettings), true)]
    public class RenderSettingsEditor : Editor
    {
        private SerializedProperty theme, addRenderer, cardActionHandlers;
        private ReorderableList cardActionHandlersList;

        private void OnEnable()
        {
            theme = serializedObject.FindProperty("theme");
            addRenderer = serializedObject.FindProperty("addMissingRenderer");
            cardActionHandlers = serializedObject.FindProperty("cardActionHandlers");

            cardActionHandlersList = Helper.GetListWithFoldout(serializedObject, cardActionHandlers, false, true, true, true);
            SetupCardActionsList();
        }

        private void SetupCardActionsList()
        {
            cardActionHandlersList = Helper.GetListWithFoldout(serializedObject, cardActionHandlers, false, false, true, true);
            cardActionHandlersList.elementHeightCallback += index =>
                cardActionHandlers.isExpanded ? EditorGUI.GetPropertyHeight(ChildOfList(cardActionHandlersList, index, "Handler")) + 20 : 0;
            cardActionHandlersList.drawElementCallback += (rect, index, active, focused) =>
            {
                if (!cardActionHandlers.isExpanded)
                    return;

                rect.height = 200;
                rect.y += 2;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    ChildOfList(cardActionHandlersList, index, "Type"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, 100),
                    ChildOfList(cardActionHandlersList, index, "Handler"), GUIContent.none);
            };
        }

        private SerializedProperty ChildOfList(ReorderableList list, int index, string name)
            => list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative(name);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(theme);
            EditorGUILayout.PropertyField(addRenderer);

            cardActionHandlersList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
