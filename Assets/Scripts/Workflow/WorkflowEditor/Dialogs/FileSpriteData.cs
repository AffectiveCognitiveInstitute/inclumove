using UnityEngine;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    /// <summary>
    ///     Stores the associated extension with a sprite.
    /// </summary>
    [System.Serializable]
    public class FileSpriteData 
    {
        public string[] extensions;
        public Sprite sprite;
    }
}