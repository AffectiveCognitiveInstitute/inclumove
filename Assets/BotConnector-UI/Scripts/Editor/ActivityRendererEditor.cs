using UnityEditor;

namespace BotConnector.Unity.UI
{
    [CustomEditor(typeof(ActivityRenderer))]
    public class ActivityRendererEditor : Editor
    {
        private SerializedProperty showInfo, wrapActivity, textPanel, statusPanel, wrapPanel;

        private void OnEnable()
        {
            showInfo = serializedObject.FindProperty(nameof(ActivityRenderer.ShowStatusInfo));
            wrapActivity = serializedObject.FindProperty(nameof(ActivityRenderer.WrapAttachment));
            textPanel = serializedObject.FindProperty(nameof(ActivityRenderer.TextPanel));
            statusPanel = serializedObject.FindProperty(nameof(ActivityRenderer.StatusPanel));
            wrapPanel = serializedObject.FindProperty(nameof(ActivityRenderer.WrapPanel));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(wrapActivity);

            if (wrapActivity.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(wrapPanel);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(showInfo);

            if (showInfo.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(statusPanel);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(textPanel);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
