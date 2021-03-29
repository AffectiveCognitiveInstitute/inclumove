using UnityEditor;
using UnityEngine;

namespace Aci.Unity.Quests
{
    [CustomPropertyDrawer(typeof(QuestContentCollection))]
    public class QuestContentPropertyDrawer : PropertyDrawer
    {
        private QuestAssetListDrawer<QuestContent> m_QuestContent;
        private bool m_IsInitialized;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!m_IsInitialized)
                Initialize(property);

            EditorGUI.BeginProperty(position, label, property);
            m_QuestContent.OnGUI(position);
            EditorGUI.EndProperty();
        }

        private void Initialize(SerializedProperty property)
        {
            m_QuestContent = new QuestAssetListDrawer<QuestContent>(property.FindPropertyRelative("m_Content"), "Quest Content", "Press the + icon to add content");
            m_IsInitialized = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (m_QuestContent == null)
                return base.GetPropertyHeight(property, label);

            return m_QuestContent.GetPropertyHeight(property, label);
        }
    }
}
