using Aci.Unity.Quests;
using UnityEditor;
using UnityEngine;

namespace Aci.Unity.Editor
{
    public class CompoundConditionDrawer 
    {
        private bool m_IsInitialized;
        private QuestAssetListDrawer<QuestCondition> m_ConditionsDrawer;

        private void Initialize(SerializedObject serializedObject)
        {
            m_IsInitialized = true;
            m_ConditionsDrawer = new QuestAssetListDrawer<QuestCondition>(serializedObject.FindProperty("m_Conditions"), "Conditions", "Click on the + icon to add conditions");
        }

        public void OnGUI(Rect position, SerializedObject serializedObject)
        {
            if (!m_IsInitialized)
                Initialize(serializedObject);
            serializedObject.Update();

            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), serializedObject.FindProperty("m_ConditionMode"));
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (serializedObject.FindProperty("m_ConditionMode").enumValueIndex == (int)QuestConditionMode.AtLeast)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), serializedObject.FindProperty("m_MinimumNumberOfConditions"));
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            m_ConditionsDrawer.OnGUI(new Rect(position.x, position.y, position.width, m_ConditionsDrawer.GetPropertyHeight(null, GUIContent.none)));

            serializedObject.ApplyModifiedProperties();
        }

        public float GetPropertyHeight(SerializedObject serializedObject)
        {
            if (!m_IsInitialized)
                Initialize(serializedObject);

            float height = EditorGUI.GetPropertyHeight(serializedObject.FindProperty("m_ConditionMode"));

            if (serializedObject.FindProperty("m_ConditionMode").enumValueIndex == (int)QuestConditionMode.AtLeast)
                height += EditorGUI.GetPropertyHeight(serializedObject.FindProperty("m_MinimumNumberOfConditions"));

            if (m_ConditionsDrawer != null)
                height += m_ConditionsDrawer.GetPropertyHeight(null, GUIContent.none);

            return height + EditorGUIUtility.singleLineHeight;
        }
    }
}