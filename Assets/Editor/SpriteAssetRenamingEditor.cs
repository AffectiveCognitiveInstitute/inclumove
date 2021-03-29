using TMPro;
namespace Aci.Unity.Editor
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class SpriteAssetRenamingEditor : EditorWindow
    {
        private TMP_SpriteAsset m_SpriteAsset;
        private string m_Path = "Assets/";

        [MenuItem("Window/TMP/SpriteAssetRenamer")]
        private static void Init()
        {
            var window = EditorWindow.GetWindow<SpriteAssetRenamingEditor>();
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            m_SpriteAsset = (TMP_SpriteAsset)EditorGUILayout.ObjectField("Sprite Asset", m_SpriteAsset, typeof(TMP_SpriteAsset), false);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Path");
            m_Path = GUILayout.TextField(m_Path);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(m_SpriteAsset == null);
            if (GUILayout.Button("Fix Names"))
                Execute();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        private void Execute()
        {
            var assets = AssetDatabase.FindAssets("t:Sprite", new string[] { m_Path });

            if(assets.Length == 0)
            {
                Debug.Log("Could not find any assets!");
                return;
            }

            for(int i = 0; i < m_SpriteAsset.spriteInfoList.Count; i++)
            {
                var info = m_SpriteAsset.spriteInfoList[i];
                var path = AssetDatabase.GUIDToAssetPath(assets[i]);
                var fileName = Path.GetFileNameWithoutExtension(path);
                try
                {
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        info.unicode = int.Parse(fileName, System.Globalization.NumberStyles.HexNumber);
                    }
                }
                catch(Exception)
                {
                    Debug.Log($"Could not replace unicode for asset ath path: {path}");
                }

            }

            m_SpriteAsset.UpdateLookupTables();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
