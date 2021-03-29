using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class FileViewController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image m_Image;

        [SerializeField]
        private TMPro.TextMeshProUGUI m_Text;

        private string m_Path;
        public string path { get => m_Path; }

        public event FileSelectedDelegate fileSelected;

        public void Initialize(string path, Sprite sprite)
        {
            m_Path = path;
            m_Image.sprite = sprite;
            m_Text.text = Path.GetFileName(path);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount < 2)
                return;

            fileSelected?.Invoke(this, new FileSelectedEventArgs()
            {
                filePath = path
            });
        }
    }
}