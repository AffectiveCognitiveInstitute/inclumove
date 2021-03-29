using Aci.Unity.Data.JsonModel;
using Aci.Unity.UI;
using Aci.Unity.UI.Dialog;
using Aci.Unity.UserInterface.Factories;
using Aci.Unity.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class ChatBotEditorDialogViewController : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<ActivityData, Action<ActivityData>, ChatBotEditorDialogViewController> { }

        [SerializeField]
        private TMPro.TMP_InputField m_InputField;

        [SerializeField]
        private TMPro.TextMeshProUGUI m_AudioPathLabel;

        [SerializeField]
        private GameObject m_NoMediaFileGameObject;

        [SerializeField]
        private GameObject m_ImageGameObject;

        [SerializeField]
        private GameObject m_VideoGameObject;

        [SerializeField]
        private CachedImageComponent m_CachedImageComponent;

        private string m_Url;
        private string m_Text;
        private string m_AudioPath;

        private ActivityData m_Payload;
        private Action<ActivityData> m_SaveAction;
        private FileSelectorFacade m_FileSelectorFacade;
        private WorkflowEditorSceneManager m_WorkflowEditorManager;
        private IConfigProvider m_ConfigProvider;
        private DialogComponent m_DialogComponent;
        private string m_MediaFilePath;
        private static readonly List<string> s_WellKnownVideoFormats = new List<string> { "*.mov", "*.mp4", "*.mkv" };
        private static readonly List<string> s_WellKnownImageFormats = new List<string> { "*.jpg", "*.jpeg", "*.png" };


        public string text
        {
            get => m_Text;
            set 
            {
                m_Text = value;
                m_InputField.SetTextWithoutNotify(value);
            }
        }

        public string audioPath
        {
            get => m_AudioPath;
            set
            {
                m_AudioPath = value;
                m_AudioPathLabel.text = string.IsNullOrWhiteSpace(value) ? "Bitte wählen Sie eine Audiodatei aus." : value;
            }
        }

        public string mediaFilePath
        {
            get => m_MediaFilePath;
            set
            {
                m_MediaFilePath = value;
                UpdateMediaPreview();
            }
        }

        [Zenject.Inject]
        private void Construct(ActivityData payload,
                               Action<ActivityData> saveAction,
                               FileSelectorFacade fileSelectorFacade,
                               WorkflowEditorSceneManager workflowEditorManager)
        {
            m_Payload = payload;
            m_SaveAction = saveAction;
            m_FileSelectorFacade = fileSelectorFacade;
            m_WorkflowEditorManager = workflowEditorManager;
        }

        private void Awake()
        {
            m_DialogComponent = GetComponent<DialogComponent>();
            // Setup data
            text = m_Payload.message;
            audioPath = m_Payload.speechFilePath;
            mediaFilePath = m_Payload.mediaFilePath;
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

        public void OnSelectAudioFileClicked()
        {
            string workflowPath = Path.Combine(m_WorkflowEditorManager.workflowDirectory, m_WorkflowEditorManager.CurrentWorkflow);
            m_FileSelectorFacade.Show((sender, args) =>
            {
                audioPath = args.filePath;
            },
            new string[] { m_WorkflowEditorManager.workflowDirectory, workflowPath }, new string[] { "*.wav" });
        }

        public void OnRemoveVideoOrImageClicked()
        {
            m_Payload.mediaFilePath = null;
            m_NoMediaFileGameObject.SetActive(true);
            m_ImageGameObject.SetActive(false);
            m_VideoGameObject.SetActive(false);
        }

        public void OnSelectVideoOrImageFileClicked()
        {
            string workflowPath = Path.Combine(m_WorkflowEditorManager.workflowDirectory, m_WorkflowEditorManager.CurrentWorkflow);
            m_FileSelectorFacade.Show((sender, args) =>
            {
                mediaFilePath = args.filePath;
                m_Payload.mediaFilePath = m_MediaFilePath;
            },
            new string[] { m_WorkflowEditorManager.workflowDirectory, workflowPath }, s_WellKnownImageFormats.Concat(s_WellKnownVideoFormats));
        }

        private void UpdateMediaPreview()
        {
            if (string.IsNullOrWhiteSpace(m_MediaFilePath))
            {
                m_NoMediaFileGameObject.SetActive(true);
                m_ImageGameObject.SetActive(false);
                m_VideoGameObject.SetActive(false);
                m_Payload.contentType = AttachmentContentType.Default;
            }
            else
            {
                string extension = $"*{Path.GetExtension(m_MediaFilePath)}";
                m_NoMediaFileGameObject.SetActive(false);

                if (s_WellKnownImageFormats.Contains(extension))
                {
                    m_VideoGameObject.SetActive(false);
                    m_ImageGameObject.SetActive(true);
                    m_CachedImageComponent.url = m_MediaFilePath;
                    m_Payload.contentType = AttachmentContentType.Material;
                }
                else
                {
                    // TODO: Improve how videos are previewed
                    m_ImageGameObject.SetActive(false);
                    m_VideoGameObject.SetActive(true);
                    m_Payload.contentType = AttachmentContentType.Video;
                }
            }            
        }

        public void OnSaveButtonClicked()
        {
            m_DialogComponent.Dismiss();

            m_SaveAction?.Invoke(new ActivityData()
            {
                message = m_Text,
                speechFilePath = m_AudioPath,
                mediaFilePath = m_MediaFilePath,
                contentType = m_Payload.contentType
            }); 
        }

        public void OnCancelButtonClicked()
        {
            m_DialogComponent.Dismiss();
        }

        public static implicit operator DialogComponent(ChatBotEditorDialogViewController vc)
        {
            return vc.m_DialogComponent ?? vc.GetComponent<DialogComponent>();
        }
    }
}