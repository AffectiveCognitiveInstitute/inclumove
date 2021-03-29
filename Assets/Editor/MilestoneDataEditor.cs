using UnityEditor;

namespace Aci.Unity.Data.Editor
{
    [CustomEditor(typeof(MilestoneData))]
    public class MilestoneDataEditor : UnityEditor.Editor
    {
        private SerializedProperty m_HasUnlockableContent;
        private SerializedProperty m_UnlockablePreview;
        private SerializedProperty m_Command;

        private void OnEnable()
        {
            m_UnlockablePreview = serializedObject.FindProperty("m_UnlockablePreview");
            m_Command = serializedObject.FindProperty("m_Command");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject);

            //EditorGUI.BeginDisabledGroup(!m_HasUnlockableContent.boolValue);
            //EditorGUILayout.PropertyField(m_UnlockablePreview);
            //EditorGUILayout.PropertyField(m_Command);
            //EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}