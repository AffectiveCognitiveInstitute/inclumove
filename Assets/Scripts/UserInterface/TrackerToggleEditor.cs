// <copyright file=TrackerToggleEditor.cs/>
// <copyright>
//   Copyright (c) 2018, Affective & Cognitive Institute
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software andassociated documentation files
//   (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
//   merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//   LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
//   IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// <license>MIT License</license>
// <main contributors>
//   Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>07/12/2018 16:08</date>

using Aci.Unity.UserInterface;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(TrackerToggle))]
public class TrackerToggleEditor : Editor
{
    private SerializedProperty manager;
    private SerializedProperty pyramid;
    private SerializedProperty timeline;
    private SerializedProperty usePyramid;
    private SerializedProperty useTimeline;

    private void OnEnable()
    {
        // Setup the SerializedProperties.
        pyramid = serializedObject.FindProperty("pyramid");
        timeline = serializedObject.FindProperty("timeline");
        useTimeline = serializedObject.FindProperty("useTimeline");
        usePyramid = serializedObject.FindProperty("usePyramid");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(pyramid);

        EditorGUILayout.PropertyField(timeline);

        Rect r = EditorGUILayout.BeginHorizontal();

        if (EditorGUILayout.ToggleLeft("Use Pyramid", usePyramid.boolValue))
        {
            usePyramid.boolValue = true;
            useTimeline.boolValue = false;
        }

        if (EditorGUILayout.ToggleLeft("Use Timeline", useTimeline.boolValue))
        {
            usePyramid.boolValue = false;
            useTimeline.boolValue = true;
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}

#endif