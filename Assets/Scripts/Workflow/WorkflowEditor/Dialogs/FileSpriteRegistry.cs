using System.Collections.Generic;
using UnityEngine;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    [CreateAssetMenu(menuName = "Scriptable Objects/File Sprite Registry")]
    public class FileSpriteRegistry : ScriptableObject
    {
        [SerializeField]
        private List<FileSpriteData> m_FileSpriteData;

        public Sprite GetSprite(string extension)
        {
            for(int i = 0; i < m_FileSpriteData.Count; i++)
            {
                FileSpriteData data = m_FileSpriteData[i];
                for(int j = 0; j < data.extensions.Length; j++)
                {
                    // Maybe do this with Regex. For now this will do...
                    if (data.extensions[j] == extension)
                        return data.sprite;
                }
            }

            return null;
        }
    }
}