using UnityEditor;
using UnityEngine;

namespace BotConnector.Unity
{
    [CustomEditor(typeof(PlayerPrefsStorage))]
    [ExecuteInEditMode]
    public class PlayerPrefsStorageEditor : Editor
    {
        private PlayerPrefsStorage storage;

        private void OnEnable()
        {
            storage = target as PlayerPrefsStorage;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Clear prefs"))
                storage.Remove();
        }

    }
}
