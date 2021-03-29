using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Aci.Unity.Quests
{
    [CustomPropertyDrawer(typeof(QuestCounterCollection))]
    public class QuestCounterCollectionPropertyDrawer : PropertyDrawer
    {
        private bool m_IsInitialized = false;
        private SerializedObject m_Object;
        private SerializedProperty m_List;
        private ReorderableList m_ReorderableList;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!m_IsInitialized)
                Initialize(property);

            EditorGUI.BeginProperty(position, label, property);

            m_ReorderableList.DoList(position);

            EditorGUI.EndProperty();
        }

        private void Initialize(SerializedProperty property)
        {
            m_Object = property.serializedObject;
            m_List = property.FindPropertyRelative("m_QuestCounters");
            m_ReorderableList = new ReorderableList(property.serializedObject, m_List)
            {
                drawHeaderCallback = OnDrawHeader,
                onAddDropdownCallback = OnAddDropdown,
                onRemoveCallback = OnRemove,
                drawElementCallback = OnDrawElement,
                elementHeightCallback = OnGetElementHeight
            };

            m_IsInitialized = true;
        }

        private void OnAddDropdown(Rect buttonRect, ReorderableList list)
        {
            IEnumerable<Type> attributeTypes = GetAllAttributeTypes<QuestCounter>();
            //Rect createBtnRect = GUILayoutUtility.GetRect(new GUIContent("Create"), EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false));
            GenericMenu menu = new GenericMenu();
            foreach (Type t in attributeTypes)
                menu.AddItem(new GUIContent(t.Name), false, () => OnMenuItemClicked(t));

            var lastRect = GUILayoutUtility.GetLastRect();

            menu.DropDown(new Rect(buttonRect.x - lastRect.width, buttonRect.y, lastRect.width, lastRect.height));

            //menu.DropDown(new Rect(buttonRect.min - new Vector2(createBtnRect.size.x, 0), createBtnRect.size));
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Quest Counters");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (m_ReorderableList == null)
                return EditorGUI.GetPropertyHeight(property, label);

            return m_ReorderableList.GetHeight();
        }

        private float OnGetElementHeight(int index)
        {
            if (m_ReorderableList.index == index)
            {
                SerializedProperty sp = m_List.GetArrayElementAtIndex(index);
                if (sp.objectReferenceValue == null)
                    return 0f;

                SerializedObject obj = new SerializedObject(sp.objectReferenceValue);
                SerializedProperty iterator = obj.GetIterator();
                float height = 0;
                bool enterChildren = true; // On first loop enter children, after do not.
                while (iterator.NextVisible(enterChildren))
                {
                    height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
                    enterChildren = false;
                }

                return height;
            }

            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
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

        private void OnMenuItemClicked(Type t)
        {
            ScriptableObject so = ScriptableObject.CreateInstance(t);

            so.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(so, m_List.serializedObject.targetObject);
            int index = m_List.arraySize;
            m_List.InsertArrayElementAtIndex(index);
            SerializedProperty sp = m_List.GetArrayElementAtIndex(index);
            SerializedObject newElement = new SerializedObject(so);
            newElement.FindProperty("m_Id").stringValue = "New Quest Counter";
            newElement.ApplyModifiedProperties();
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

            SerializedProperty sp = obj.FindProperty("m_Id");
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), sp.stringValue);

            if (isActive || isFocused)
            {
                EditorGUI.BeginChangeCheck();
                obj.Update();

                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                SerializedProperty iterator = obj.GetIterator();
                EditorGUI.indentLevel++;
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

                obj.ApplyModifiedProperties();
                EditorGUI.EndChangeCheck();
                EditorGUI.indentLevel--;
            }
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