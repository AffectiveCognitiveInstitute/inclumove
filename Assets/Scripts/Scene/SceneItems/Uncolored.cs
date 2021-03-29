using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.Scene.SceneItems
{
    class Uncolored : MonoBehaviour, IColorable
    {
        static Color transparent = new Color(0,0,0,0);
        public Color color
        {
            get => transparent;
            set { }
        }
    }
}
