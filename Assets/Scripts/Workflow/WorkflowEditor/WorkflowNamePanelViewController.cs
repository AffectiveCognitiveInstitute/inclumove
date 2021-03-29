using System.IO;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class WorkflowNamePanelViewController : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_InputField m_InputField;

        private WorkflowEditorSceneManager m_EditorSceneManager;

        [Inject]
        private void Construct(WorkflowEditorSceneManager editorSceneManager)
        {
            m_EditorSceneManager = editorSceneManager;
        }

        private void Awake()
        {
            m_EditorSceneManager.workflowLoaded.AddListener(OnSceneNameConfirmed);
        }

        private void OnDestroy()
        {
            m_EditorSceneManager.workflowLoaded.RemoveListener(OnSceneNameConfirmed);
        }

        public void OnSceneNameConfirmed()
        {
            string newScenePath = Path.Combine(m_EditorSceneManager.workflowDirectory, m_InputField.text + ".work");
            string oldScenePath = Path.Combine(m_EditorSceneManager.workflowDirectory, m_EditorSceneManager.CurrentWorkflow + ".work");
            if (!m_EditorSceneManager.IntializeWorkflow(oldScenePath, newScenePath))
                m_InputField.text = m_EditorSceneManager.CurrentWorkflow;
        }
    }
}