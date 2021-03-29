using UnityEngine;
using UnityEditor;
using Zenject;
using System.Collections.Generic;
using Aci.Unity.UI.Dialog;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class NewFileDialogViewController : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<FileSelectedDelegate, string, string, NewFileDialogViewController> { }

        [SerializeField]
        private TMPro.TextMeshProUGUI m_FilePathPreviewLabel;

        [SerializeField]
        private TMPro.TMP_InputField m_InputField;

        private string m_TargetPath;
        private string m_FileType;
        private string m_FilePath;
        private DialogComponent m_DialogComponent;

        public event FileSelectedDelegate fileSelected;

        [Zenject.Inject]
        private void Construct(FileSelectedDelegate fileSelected, string targetPath, string fileType)
        {
            m_TargetPath = targetPath;
            m_FileType = fileType.Remove(0,1);
            m_FilePath = m_TargetPath + "/" + m_FileType;
            m_FilePathPreviewLabel.text = m_FilePath;
            this.fileSelected += fileSelected;
        }

        private void Awake()
        {
            m_DialogComponent = GetComponent<DialogComponent>();
        }

        private void OnEnable()
        {
            m_DialogComponent.dismissed += OnDialogDismissed;

            // Hack for input field to scale correctly. Not quite sure what's going on here
            m_InputField.gameObject.SetActive(false);
            m_InputField.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            m_DialogComponent.dismissed -= OnDialogDismissed;
        }

        private void OnDialogDismissed(IDialog dialog)
        {
            Destroy(gameObject);
        }

        public void OnFileNameChanged(string newName)
        {
            m_FilePath = m_TargetPath + "/" + newName + m_FileType;
            m_FilePathPreviewLabel.text = m_FilePath;
        }

        public void OnSaveButtonClicked()
        {
            m_DialogComponent.Dismiss();
            fileSelected?.Invoke(this, new FileSelectedEventArgs() { filePath = m_FilePath });
        }

        public void OnCancelButtonClicked()
        {
            m_DialogComponent.Dismiss();
        }

        public static implicit operator DialogComponent(NewFileDialogViewController vc)
        {
            return vc.m_DialogComponent ?? vc.GetComponent<DialogComponent>();
        }
    }
}