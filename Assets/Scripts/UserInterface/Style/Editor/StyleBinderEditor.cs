

namespace Aci.Unity.UserInterface.Style
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;

    [CustomEditor(typeof(StyleBinder))]
    public class StyleBinderEditor : Editor
    {
        private string[] m_AttributeNames;
        private string[] m_TargetPropertyNames;

        private Style m_Style;
        private SerializedProperty m_StyleProperty;
        private SerializedProperty m_AttributeName;
        private SerializedProperty m_TargetProperty;
        private SerializedProperty m_PropertyName;
        private List<PropertyInfo> m_TargetProperties;

        private void OnEnable()
        {
            m_StyleProperty = serializedObject.FindProperty("m_Style");
            m_AttributeName = serializedObject.FindProperty("m_AttributeName");
            m_TargetProperty = serializedObject.FindProperty("m_Target");
            m_PropertyName = serializedObject.FindProperty("m_PropertyName");
        }

        private void Invalidate()
        {
            m_Style = (target as StyleBinder).style;

            UpdateAttributeNames();
            UpdatePropertyNames();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_StyleProperty);
            if (EditorGUI.EndChangeCheck())
                Invalidate();

            if (m_StyleProperty == null)
            {
                EditorGUILayout.HelpBox("Could not find IStyle in the ProjectContext", MessageType.Error);
                return;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_TargetProperty);
            if (EditorGUI.EndChangeCheck())
                Invalidate();

            if(m_AttributeNames != null)
            {
                string attributeName = m_AttributeName.stringValue;
                int index = Array.IndexOf(m_AttributeNames, attributeName);
                index = EditorGUILayout.Popup(m_AttributeName.displayName, index, m_AttributeNames);
                if (index != -1)
                    m_AttributeName.stringValue = m_AttributeNames[index];
            }

            if(m_TargetPropertyNames != null && !string.IsNullOrWhiteSpace(m_AttributeName.stringValue))
            {
                string targetPropertyName = m_PropertyName.stringValue;
                int index = Array.IndexOf(m_TargetPropertyNames, targetPropertyName);
                index = EditorGUILayout.Popup(m_PropertyName.displayName, index, m_TargetPropertyNames);
                if (index != -1)
                    m_PropertyName.stringValue = m_TargetPropertyNames[index];
            }


            serializedObject.ApplyModifiedProperties();

        }

        private void UpdatePropertyNames()
        {
            if (m_TargetProperty.objectReferenceValue == null) return;

            object target = m_TargetProperty.objectReferenceValue;

            var tempTarget = from targetProp in target.GetType().GetProperties(BindingFlags.FlattenHierarchy | 
                                                                               BindingFlags.Public |
                                                                               BindingFlags.Instance)
                             select targetProp;

            m_TargetProperties = tempTarget.ToList();
            m_TargetPropertyNames = m_TargetProperties.Select(p => p.Name).ToArray();
        }

        private void UpdateAttributeNames()
        {
            if (m_Style == null)
            {
                m_AttributeNames = null;
                return;
            }

            m_AttributeNames = new string[m_Style.count];

            for (int i = 0; i < m_Style.count; i++)
                m_AttributeNames[i] = m_Style[i].attributeName;
        }
    }
}