using UnityEditor;
using UnityEngine;

namespace Aci.Unity.UserInterface
{
    [CustomEditor(typeof(UINumberScroller))]
    public class UINumberScrollerEditor : UnityEditor.Editor
    {
        private UINumberScroller m_Target;

        private void OnEnable()
        {
            m_Target = target as UINumberScroller;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });            
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            {

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Target Value");
                    if (GUILayout.Button("-"))
                        m_Target.targetValue = m_Target.targetValue - 1;

                    m_Target.targetValue = EditorGUILayout.DelayedIntField(m_Target.targetValue, GUILayout.MaxWidth(100));

                    if (GUILayout.Button("+"))
                        m_Target.targetValue = m_Target.targetValue + 1;
                }

                EditorGUILayout.EndHorizontal();

            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

        }
    }
}