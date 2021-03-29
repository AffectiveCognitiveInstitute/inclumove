using System;
using System.Collections.Generic;

namespace Aci.Unity.UserInterface.Style
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Style))]
    public class StyleEditor : Editor
    {
        private List<int> m_ItemsToDelete = new List<int>();
        private SerializedProperty m_Attributes;

        private void OnEnable()
        {
            m_Attributes = serializedObject.FindProperty("m_Attributes");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawAttributes();

            EditorGUILayout.EndVertical();


            serializedObject.ApplyModifiedProperties();
        }

        private void ShowAttributeMenu()
        {
            IEnumerable<Type> attributeTypes = GetAllAttributeTypes<OverridableAttribute>();
            Rect createBtnRect = GUILayoutUtility.GetRect(new GUIContent("Create"), EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false));
            GenericMenu menu = new GenericMenu();
            foreach (Type t in attributeTypes)
                menu.AddItem(new GUIContent(t.Name), false, () => OnMenuItemClicked(t));

            menu.DropDown(new Rect(Event.current.mousePosition, createBtnRect.size));
        }

        private void OnMenuItemClicked(Type t)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(t);

            so.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(so, serializedObject.targetObject);
            int index = m_Attributes.arraySize;
            m_Attributes.InsertArrayElementAtIndex(index);
            SerializedProperty sp = m_Attributes.GetArrayElementAtIndex(index);
            sp.objectReferenceValue = so;
            AssetDatabase.SaveAssets();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAttributes()
        {
            EditorGUILayout.LabelField("Attributes", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Attribute Name");
            EditorGUILayout.LabelField("Value");
            EditorGUILayout.EndHorizontal();

            m_ItemsToDelete.Clear();

            int count = m_Attributes.arraySize;

            if(count == 0)
            {
                EditorGUILayout.HelpBox("There are currently no attributes added to this Style.", MessageType.Info);
            }

            for (int i = 0; i < count; i++)
            {
                SerializedProperty s = m_Attributes.GetArrayElementAtIndex(i);
                if (DrawAttributeElement(s))
                    m_ItemsToDelete.Add(i);
                EditorGUILayout.Space();
            }


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Attribute", EditorStyles.toolbarDropDown, GUILayout.MaxWidth(100)))
            {
                ShowAttributeMenu();
            }
            EditorGUILayout.EndHorizontal();

            // Delete any objects
            foreach (int index in m_ItemsToDelete)
            {
                SerializedProperty s = m_Attributes.GetArrayElementAtIndex(index);
                AssetDatabase.RemoveObjectFromAsset(s.objectReferenceValue);
                s.objectReferenceValue = null;
                m_Attributes.DeleteArrayElementAtIndex(index);
            }

            if (m_ItemsToDelete.Count > 0)
                serializedObject.ApplyModifiedProperties();
        }

        private bool DrawAttributeElement(SerializedProperty serializedProperty)
        {
            SerializedObject so = new SerializedObject(serializedProperty.objectReferenceValue);
            so.Update();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(so.FindProperty("m_AttributeName"), GUIContent.none);

            //EditorGUILayout.PropertyField(so.FindProperty("m_Key"), GUIContent.none);
            EditorGUILayout.PropertyField(so.FindProperty("m_Value"), GUIContent.none, GUILayout.MinWidth(10));
            GUIContent icon = EditorGUIUtility.IconContent("Toolbar Minus");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(icon, GUIStyle.none))
                return true;
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
                so.ApplyModifiedProperties();

            return false;
        }

        private IEnumerable<Type> GetAllAttributeTypes<T>() where T : class
        {
            var subtypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("Mono.Cecil")) continue;
                if (assembly.FullName.StartsWith("UnityScript")) continue;
                if (assembly.FullName.StartsWith("Boo.Lan")) continue;
                if (assembly.FullName.StartsWith("System")) continue;
                if (assembly.FullName.StartsWith("I18N")) continue;
                if (assembly.FullName.StartsWith("UnityEngine")) continue;
                if (assembly.FullName.StartsWith("UnityEditor")) continue;
                if (assembly.FullName.StartsWith("mscorlib")) continue;
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsClass) continue;
                    if (type.IsAbstract) continue;
                    if (!type.IsSubclassOf(typeof(T))) continue;
                    subtypes.Add(type);
                }
            }

            return subtypes;
        }
    }
}
