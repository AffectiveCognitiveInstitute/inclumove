using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BotConnector.Unity.UI
{
    [CustomEditor(typeof(Chat))]
    public class ChatEditor : Editor
    {
        private ReorderableList inputsList;
        private SerializedProperty chatPanel, suggestedActionsContainer, suggestedActionsContent, inputs;

        private void OnEnable()
        {
            chatPanel = serializedObject.FindProperty("chatPanel");
            suggestedActionsContainer = serializedObject.FindProperty("suggestedActionsContainer");
            suggestedActionsContent = serializedObject.FindProperty("suggestedActionsContent");
            inputs = serializedObject.FindProperty("inputs");

            SetupInputList();
        }

        private void SetupInputList()
        {
            inputsList = Helper.GetListWithFoldout(serializedObject, inputs, false, false, true, true);
            inputsList.drawElementCallback += (rect, index, active, focused) =>
            {
                if (!inputs.isExpanded)
                    return;

                rect.height = 200;
                rect.y += 2;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    inputs.GetArrayElementAtIndex(index), GUIContent.none);
            };
            inputsList.elementHeightCallback += index => inputs.isExpanded ? Helper.SingleLineHeight : 0;
        }

        private SerializedProperty ChildOfList(ReorderableList list, int index, string name)
            => list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative(name);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(5);

            EditorGUILayout.PropertyField(chatPanel);
            EditorGUILayout.PropertyField(suggestedActionsContainer);
            EditorGUILayout.PropertyField(suggestedActionsContent);

            GUILayout.Space(5);

            inputsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }

}
