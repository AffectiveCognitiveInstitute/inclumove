using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BotDebugger))]
[CanEditMultipleObjects]
public class BotDebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Send message"))
        {
            var script = (BotDebugger)target;
            script.SendMessageToBot();
        }
    }
}
