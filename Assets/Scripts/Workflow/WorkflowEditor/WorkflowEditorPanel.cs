// <copyright file=WorkflowEditorPanel.cs/>
// <copyright>
//   Copyright (c) 2019, Affective & Cognitive Institute
// 
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software andassociated documentation files
//   (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
//   merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
// 
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//   LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
//   IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <license>MIT License</license>
// <main contributors>
//   Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>10/24/2019 09:20</date>

using System.Collections.Generic;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Scene.SceneItems;
using Aci.Unity.Util;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class WorkflowEditorPanel : MonoBehaviour
    {
        private IConfigProvider m_ConfigProvider;

        private WorkflowEditorSceneManager m_EditorSceneManager;

        private FileSelectorFacade m_FileSelectorFacade;

        private NewFileDialogFacade m_NewFileDialogFacade;

        [SerializeField] private ReorderableList m_List;

        [SerializeField] private GameObject m_ListElementPrefab;

        private ISceneService m_SceneService;

        public IStepItem activeStep { get; private set; }

        [Inject]
        private void Construct(ISceneService    sceneService,    WorkflowEditorSceneManager editorSceneManager,
                               StepItem.Factory stepItemFactory, WorkflowData.Factory       workflowDataFactory,
                               FileSelectorFacade fileSelectorFacade, NewFileDialogFacade newFileDialogFacade)
        {
            m_SceneService = sceneService;
            m_EditorSceneManager = editorSceneManager;
            m_FileSelectorFacade = fileSelectorFacade;
            m_NewFileDialogFacade = newFileDialogFacade;

            m_EditorSceneManager.stepsChanged.AddListener(OnStepsChanged);
            m_EditorSceneManager.activeStepChanged.AddListener(OnActiveStepChanged);
        }

        private void OnDestroy()
        {
            m_ConfigProvider?.UnregisterClient(this);
        }

        public void Unregister(StepItemElementViewController controller)
        {
            int index = GetIndex(controller);
            if (index == -1)
                return;
            m_EditorSceneManager.RemoveStepAt(index);
        }

        public void AddButtonPressed()
        {
            m_EditorSceneManager.AddStep(WorkflowStepData.Empty);
        }

        public void SetActive(StepItemElementViewController controller)
        {
            int index = GetIndex(controller);
            if (index == -1)
                return;
            m_EditorSceneManager.SetActiveStep(index);
        }

        public void OnElementMoved(ReorderableList.ReorderableListEventStruct e)
        {
            GameObject.Destroy(e.DroppedObject);
            m_EditorSceneManager.MoveStepFromTo(e.FromIndex, e.ToIndex);
        }

        private void OnActiveStepChanged(int index)
        {
            StepItemElementViewController[] controllers =
                m_List.Content.GetComponentsInChildren<StepItemElementViewController>();
            foreach (StepItemElementViewController controller in controllers) controller.OnActiveChanged(false);

            if (index == -1)
                return;

            m_List.Content.GetChild(index)?.GetComponent<StepItemElementViewController>()?.OnActiveChanged(true);
        }

        private void Clear()
        {
            while (m_List.Content.childCount > 0)
            {
                Transform trans = m_List.Content.GetChild(0);
                trans.parent = null;
                Destroy(trans.gameObject);
            }
        }

        private void OnStepsChanged(IEnumerable<IStepItem> items)
        {
            Clear();

            foreach (IStepItem step in items)
            {
                StepItemElementViewController controller = Instantiate(m_ListElementPrefab, m_List.Content, false)
                    .GetComponent<StepItemElementViewController>();
                controller.Inititalize(this, step);
            }
        }

        private int GetIndex(StepItemElementViewController controller)
        {
            int i = 0;
            for (; i < controller.transform.parent.childCount; ++i)
                if (controller.transform.parent.GetChild(i) == controller.transform)
                    break;
            return i < controller.transform.parent.childCount ? i : -1;
        }

        public void SaveCurrentWorkflow()
        {
            m_EditorSceneManager.WriteWorkflow();
        }

        public void LoadWorkflow()
        {
            m_FileSelectorFacade.Show((sender, args) => 
            {
                m_EditorSceneManager.LoadWorkflow(args.filePath);
            }
            , new string[] { m_EditorSceneManager.workflowDirectory }, new string[] { "*.work" });
        }

        public void NewWorkflow()
        {
            m_NewFileDialogFacade.Show((sender, args) =>
            {
                if(m_EditorSceneManager.IntializeWorkflow("", args.filePath))
                    m_EditorSceneManager.ClearItems();
            }
            , m_EditorSceneManager.workflowDirectory, "*.work");
        }

        public void Quit()
        {
            m_SceneService.SwitchScene("scn_WorkflowEditor", "scn_CUI_single");
        }
    }
}