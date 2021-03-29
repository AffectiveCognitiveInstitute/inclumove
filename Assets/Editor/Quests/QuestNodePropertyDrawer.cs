using UnityEditor;
using UnityEngine;

namespace Aci.Unity.Quests
{
    [CustomPropertyDrawer(typeof(QuestNode))]
    public class QuestNodePropertyDrawer : PropertyDrawer
    {
        private bool m_IsInitialized = false;
        private QuestAssetListDrawer<QuestCondition> m_ConditionsDrawer;
        private QuestAssetListDrawer<QuestTrigger> m_SuccessTriggersDrawer;
        private QuestAssetListDrawer<QuestTrigger> m_FailedTriggersDrawer;
        private float m_Height;

        private void Initialize(SerializedProperty property)
        {
            m_IsInitialized = true;
            m_ConditionsDrawer = new QuestAssetListDrawer<QuestCondition>(property.FindPropertyRelative("m_Conditions"), "Conditions", "Click on the + icon to add conditions");
            m_SuccessTriggersDrawer = new QuestAssetListDrawer<QuestTrigger>(property.FindPropertyRelative("m_SuccessTriggers"), "Success Triggers", "Click on the + icon to add a trigger");
            m_FailedTriggersDrawer = new QuestAssetListDrawer<QuestTrigger>(property.FindPropertyRelative("m_FailedTriggers"), "Fail Triggers", "Click on the + icon to add a trigger");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!m_IsInitialized)
                Initialize(property);

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("m_ConditionMode"));
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if(property.FindPropertyRelative("m_ConditionMode").enumValueIndex == (int) QuestConditionMode.AtLeast)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("m_MinimumNumberOfConditions"));
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            m_ConditionsDrawer.OnGUI(new Rect(position.x, position.y, position.width, m_ConditionsDrawer.GetPropertyHeight(property, label)));
            position.y += m_ConditionsDrawer.GetPropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing;
            m_SuccessTriggersDrawer.OnGUI(new Rect(position.x, position.y, position.width, m_SuccessTriggersDrawer.GetPropertyHeight(property, label)));
            position.y += m_SuccessTriggersDrawer.GetPropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing;
            m_FailedTriggersDrawer.OnGUI(new Rect(position.x, position.y, position.width, m_FailedTriggersDrawer.GetPropertyHeight(property, label)));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_ConditionMode"));

            if (property.FindPropertyRelative("m_ConditionMode").enumValueIndex == (int)QuestConditionMode.AtLeast)
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_MinimumNumberOfConditions"));

            if (m_ConditionsDrawer != null)
                height += m_ConditionsDrawer.GetPropertyHeight(property, label);

            if (m_SuccessTriggersDrawer != null)
                height += m_SuccessTriggersDrawer.GetPropertyHeight(property, label);

            if (m_FailedTriggersDrawer != null)
                height += m_FailedTriggersDrawer.GetPropertyHeight(property, label);

            return height + EditorGUIUtility.singleLineHeight;
        }       
    }
}
