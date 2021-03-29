using Aci.Unity.UserInterface.Style;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Aci.Unity.Data
{
    [CustomEditor(typeof(OverrideStyleAttributeCommand))]
    public class OverrideStyleAttributeCommandEditor : UnityEditor.Editor
    {
        private SerializedProperty m_Style;
        private SerializedProperty m_Overrides;

        private List<string> m_AvailableOverrides = new List<string>();
        private ReorderableList m_List;
        private int m_SelectedIndex;

        private void OnEnable()
        {
            m_Style = serializedObject.FindProperty("m_Style");
            m_Overrides = serializedObject.FindProperty("m_Overrides");
            UpdateAvailableOverrides();

            m_List = new ReorderableList(serializedObject, m_Overrides, false, true, true, true)
            {
                elementHeightCallback = OnGetElementHeight,
                drawHeaderCallback = OnDrawHeader,
                onAddDropdownCallback = OnDropdownCallback,
                drawElementCallback = OnDrawElement,
                onRemoveCallback = OnRemoveCallback,
                onCanAddCallback = OnCanAddCallback
            };
        }

        private bool OnCanAddCallback(ReorderableList list)
        {
            return m_AvailableOverrides.Count > 0;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Overrides");
        }

        private void OnDropdownCallback(Rect buttonRect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < m_AvailableOverrides.Count; i++)
                menu.AddItem(new GUIContent(m_AvailableOverrides[i]), false, OnItemAdded, m_AvailableOverrides[i]);

            menu.ShowAsContext();
        }

        private void OnItemAdded(object userData)
        {
            string attributeName = (string)userData;
            Style style = (Style) m_Style.objectReferenceValue;
            if(style.TryGetAttribute(attributeName, out OverridableAttribute attribute))
            {
                OverridableAttribute copy = Instantiate(attribute);
                copy.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(copy, serializedObject.targetObject);
                AssetDatabase.SaveAssets();
                m_Overrides.InsertArrayElementAtIndex(m_Overrides.arraySize);
                SerializedProperty prop = m_Overrides.GetArrayElementAtIndex(m_Overrides.arraySize - 1);
                prop.objectReferenceValue = copy;
                serializedObject.ApplyModifiedProperties();

                UpdateAvailableOverrides();
            }
        }


        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty property = m_Overrides.GetArrayElementAtIndex(index);

            if (property.objectReferenceValue == null)
                return;

            SerializedObject obj = new SerializedObject(property.objectReferenceValue);
            obj.Update();
            SerializedProperty attributeName = obj.FindProperty("m_AttributeName");
            SerializedProperty value = obj.FindProperty("m_Value");

            float attributeNameHeight = EditorGUI.GetPropertyHeight(attributeName, true);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, attributeNameHeight), attributeName);
            EditorGUI.EndDisabledGroup();
            float valueHeight = EditorGUI.GetPropertyHeight(value, true);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + attributeNameHeight + EditorGUIUtility.standardVerticalSpacing, rect.width, valueHeight), value);
            obj.ApplyModifiedProperties();
        }


        private void OnRemoveCallback(ReorderableList list)
        {
            SerializedProperty serializedProperty = m_Overrides.GetArrayElementAtIndex(list.index);
            if (serializedProperty != null)
            {
                UnityEngine.Object internalObject = serializedProperty.objectReferenceValue;
                AssetDatabase.RemoveObjectFromAsset(internalObject);
                DestroyImmediate(internalObject, true);
                serializedProperty.objectReferenceValue = null;
                m_Overrides.DeleteArrayElementAtIndex(list.index);
                m_Overrides.serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();

                UpdateAvailableOverrides();
            }
        }


        private float OnGetElementHeight(int index)
        {
            SerializedProperty sp = m_Overrides.GetArrayElementAtIndex(index);
            if (sp.objectReferenceValue == null)
                return 0f;

            SerializedObject obj = new SerializedObject(sp.objectReferenceValue);
            SerializedProperty iterator = obj.GetIterator();
            float height = 0;
            bool enterChildren = true; // On first loop enter children, after do not.
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.displayName == "Script")
                    continue;

                height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
                enterChildren = false;
            }

            return height;
        }

        private void UpdateAvailableOverrides()
        {
            Style style = (Style) m_Style.objectReferenceValue;

            if (style == null)
                return;

            m_AvailableOverrides.Clear();

            List<string> usedAttributes = new List<string>();
            for(int i = 0; i < m_Overrides.arraySize; i++)
            {
                OverridableAttribute attribute = (OverridableAttribute) m_Overrides.GetArrayElementAtIndex(i).objectReferenceValue;
                usedAttributes.Add(attribute.attributeName);
            }

            foreach(OverridableAttribute attribute in style)
            {
                if (!m_AvailableOverrides.Contains(attribute.attributeName) && !usedAttributes.Contains(attribute.attributeName))
                    m_AvailableOverrides.Add(attribute.attributeName);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Style);
            if (EditorGUI.EndChangeCheck())
                UpdateAvailableOverrides();

            if(m_Style.objectReferenceValue != null)
                m_List.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawOverrides()
        {
            for (int i = 0; i < m_Overrides.arraySize; i++)
            {
                SerializedProperty property = m_Overrides.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(property);
            }
        }
    }
}
