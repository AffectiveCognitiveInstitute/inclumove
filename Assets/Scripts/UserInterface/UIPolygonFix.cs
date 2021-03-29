using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface
{
    public class UIPolygonFix : MonoBehaviour
    {
        [SerializeField]
        private Graphic m_Graphic;

        private async void Start()
        {
            await Task.Delay(40);
            if (m_Graphic != null)
                m_Graphic.SetAllDirty();
        }

        private void OnValidate()
        {
            m_Graphic = GetComponent<Graphic>();
        }
    }
}