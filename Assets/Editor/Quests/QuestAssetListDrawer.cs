using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections.Generic;
using Aci.Unity.Editor;

namespace Aci.Unity.Quests
{
    public class QuestAssetListDrawer<T>
    {
        private string m_Header;
        private string m_NoContentText;
        private SerializedProperty m_List;
        private ReorderableList m_ReorderableList;
        private CompoundConditionDrawer m_CompoundConditionDrawer;
        public QuestAssetListDrawer(SerializedProperty property, string header, string noContentText)
        {
            m_Header = header;
            m_NoContentText = noContentText;
            m_CompoundConditionDrawer = new CompoundConditionDrawer();
            m_List = property;
            m_ReorderableList = new ReorderableList(property.serializedObject, m_List)
            {
                drawHeaderCallback = OnDrawHeader,
                drawElementCallback = OnDrawElement,
                elementHeightCallback = OnGetElementHeight,
                onAddDropdownCallback = OnAddDropdown,
                drawNoneElementCallback = OnDrawNone,
                onRemoveCallback = OnRemove
            };
        }

        public void OnGUI(Rect position)
        {
            m_ReorderableList.DoList(position);
        }

        private float OnGetElementHeight(int index)
        {
            if (m_ReorderableList.index == index)
            {
                SerializedProperty sp = m_List.GetArrayElementAtIndex(index);
                if (sp.objectReferenceValue == null)
                    return 0f;

                float height = 0;
                // TODO: How to draw property drawer? Does not work with EditorGUI.PropertyField
                if (sp.objectReferenceValue.GetType() == typeof(CompoundCondition))
                {
                    height = m_CompoundConditionDrawer.GetPropertyHeight(new SerializedObject(sp.objectReferenceValue)) + EditorGUIUtility.singleLineHeight;
                }
                else
                {

                    SerializedObject obj = new SerializedObject(sp.objectReferenceValue);
                    SerializedProperty iterator = obj.GetIterator();
                    bool enterChildren = true; // On first loop enter children, after do not.
                    while (iterator.NextVisible(enterChildren))
                    {
                        height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
                        enterChildren = false;
                    }
                }

                return height;
            }

            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private void OnDrawNone(Rect rect)
        {
            EditorGUI.LabelField(rect, m_NoContentText);
        }

        private bool OnCanRemoveCondition(ReorderableList list)
        {
            return true;
        }

        private void OnRemove(ReorderableList list)
        {
            SerializedProperty serializedProperty = m_List.GetArrayElementAtIndex(list.index);
            if (serializedProperty != null)
            {
                UnityEngine.Object internalObject = serializedProperty.objectReferenceValue;
                AssetDatabase.RemoveObjectFromAsset(internalObject);
                UnityEngine.Object.DestroyImmediate(internalObject, true);
                serializedProperty.objectReferenceValue = null;
                m_List.DeleteArrayElementAtIndex(list.index);
                m_List.serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }
        }

        private void OnAddDropdown(Rect buttonRect, ReorderableList list)
        {
            IEnumerable<Type> attributeTypes = GetAllAttributeTypes<T>();
            //Rect createBtnRect = GUILayoutUtility.GetRect(new GUIContent("Create"), EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false));
            GenericMenu menu = new GenericMenu();
            foreach (Type t in attributeTypes)
                menu.AddItem(new GUIContent(t.Name), false, () => OnMenuItemClicked(t));

            var lastRect = GUILayoutUtility.GetLastRect();

            menu.DropDown(new Rect(buttonRect.x - lastRect.width, buttonRect.y, lastRect.width, lastRect.height));

            //menu.DropDown(new Rect(buttonRect.min - new Vector2(createBtnRect.size.x, 0), createBtnRect.size));
        }

        private void OnMenuItemClicked(Type t)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(t);

            so.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(so, m_List.serializedObject.targetObject);
            int index = m_List.arraySize;
            m_List.InsertArrayElementAtIndex(index);
            SerializedProperty sp = m_List.GetArrayElementAtIndex(index);
            sp.objectReferenceValue = so;
            AssetDatabase.SaveAssets();
            m_List.serializedObject.ApplyModifiedProperties();
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty conditionElement = m_List.GetArrayElementAtIndex(index);

            if (conditionElement.objectReferenceValue == null)
                return;

            SerializedObject obj = new SerializedObject(conditionElement.objectReferenceValue);
            SerializedProperty sp = obj.FindProperty("m_Script");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), sp, GUIContent.none);
            EditorGUI.EndDisabledGroup();



            if (isActive || isFocused)
            {
                EditorGUI.BeginChangeCheck();
                obj.Update();

                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.indentLevel++;

                // TODO: How to draw property drawer? Does not work with EditorGUI.PropertyField
                if (conditionElement.objectReferenceValue.GetType() == typeof(CompoundCondition))
                {
                    float height = m_CompoundConditionDrawer.GetPropertyHeight(obj) + EditorGUIUtility.standardVerticalSpacing;
                    m_CompoundConditionDrawer.OnGUI(new Rect(rect.x, rect.y, rect.width, height), obj);
                }
                else
                {
                    SerializedProperty iterator = obj.GetIterator();
                    bool enterChildren = true;
                    while (iterator.NextVisible(enterChildren))
                    {
                        if (iterator.propertyPath == "m_Script")
                            continue;

                        float height = EditorGUI.GetPropertyHeight(iterator, true);
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), iterator, true);
                        rect.y += height + EditorGUIUtility.standardVerticalSpacing;
                        enterChildren = false;
                    }
                }

                obj.ApplyModifiedProperties();
                EditorGUI.EndChangeCheck();
                EditorGUI.indentLevel--;
            }
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, m_Header);
        }


        public float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (m_ReorderableList == null)
                return EditorGUI.GetPropertyHeight(property, label);

            return m_ReorderableList.GetHeight();
        }

        private IEnumerable<Type> GetAllAttributeTypes<T>()
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
