// <copyright file=WorkflowEditorSceneManager.cs/>
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
// <date>10/24/2019 14:30</date>

using System.Collections.Generic;
using System.IO;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Scene;
using Aci.Unity.Scene.SceneItems;
using Aci.Unity.Util;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class WorkflowEditorSceneManager : MonoBehaviour
    {
        private IConfigProvider    m_ConfigProvider;
        private ISceneItemRegistry m_SceneItemRegistry;
        private StepItem.Factory   m_StepFactory;

        [SerializeField] private GameObject m_StepItemPrefab;

        private readonly List<WorkflowPartData> m_PartData = new List<WorkflowPartData>();
        private readonly List<IStepItem>        m_StepItems = new List<IStepItem>();
        private          WorkflowData.Factory   m_WorkflowDataFactory;
        private          string                 m_CurrentWorkflow;

        [ConfigValue("workflowDirectory")]
        public string workflowDirectory { get; set; } = "";

        public string CurrentWorkflow => m_CurrentWorkflow;

        public IStepItem activeStep { get; private set; }

        public UnityEvent<IReadOnlyList<IStepItem>> stepsChanged      { get; } = new WorkflowStepsChanged();
        public UnityEvent<int>                      activeStepChanged { get; } = new ActiveStepChanged();
        public UnityEvent                           workflowLoaded    { get; } = new UnityEvent();

        [Inject]
        private void Construct(IConfigProvider  configProvider,  ISceneItemRegistry   sceneItemRegistry,
                               StepItem.Factory stepItemFactory, WorkflowData.Factory workflowDataFactory)
        {
            m_ConfigProvider = configProvider;
            m_SceneItemRegistry = sceneItemRegistry;
            m_StepFactory = stepItemFactory;
            m_WorkflowDataFactory = workflowDataFactory;

            m_ConfigProvider?.RegisterClient(this);
            //write default values to config if no config values were loaded
            if (workflowDirectory.IsNullOrEmpty())
                m_ConfigProvider?.ClientDirty(this);

            m_SceneItemRegistry.itemAdded.AddListener(OnSceneItemRegistered);
            m_SceneItemRegistry.itemRemoved.AddListener(OnSceneItemRemoved);
        }

        private void AdjustPayloadPaths(string oldPath, string newPath)
        {
            foreach(StepItem step in m_StepItems)
            {
                foreach(SceneItem item in step.sceneItems)
                {
                    string payload = item.payloadViewController.payload;
                    payload = payload.Replace(oldPath, newPath);
                    item.payloadViewController.SetPayload(item.payloadViewController.payloadType, payload, item.payloadViewController.delay);
                }
            }
        }

        public void OnDestroy()
        {
            m_SceneItemRegistry.itemAdded.RemoveListener(OnSceneItemRegistered);
            m_SceneItemRegistry.itemRemoved.RemoveListener(OnSceneItemRemoved);
        }

        public void AddStep(WorkflowStepData data)
        {
            IStepItem stepItem = m_StepFactory.Create(data, m_StepItemPrefab);
            m_StepItems.Add(stepItem);
            stepsChanged?.Invoke(m_StepItems);
        }

        public void RemoveStepAt(int index)
        {
            if (index == -1 || index >= m_StepItems.Count)
                return;
            IStepItem item = m_StepItems[index];
            if (item == activeStep)
                activeStep = null;
            m_StepItems.RemoveAt(index);
            item.Dispose();
            stepsChanged.Invoke(m_StepItems);
        }

        public void MoveStepFromTo(int sourceIndex, int targetIndex)
        {
            if (sourceIndex == targetIndex
                || sourceIndex == -1
                || sourceIndex >= m_StepItems.Count
                || targetIndex == -1
                || targetIndex >= m_StepItems.Count)
                return;
            IStepItem item = m_StepItems[sourceIndex];
            m_StepItems.RemoveAt(sourceIndex);
            m_StepItems.Insert(targetIndex, item);
            stepsChanged.Invoke(m_StepItems);
        }

        public void SetActiveStep(int index)
        {
            if (activeStep != null)
                foreach (ISceneItem sceneItem in activeStep.sceneItems)
                    sceneItem.itemTransform.gameObject.SetActive(false);

            IStepItem item = m_StepItems[index];
            activeStep = item;

            foreach (ISceneItem sceneItem in activeStep.sceneItems) sceneItem.itemTransform.gameObject.SetActive(true);

            activeStepChanged.Invoke(index);
        }

        public void OnSceneItemRegistered(ISceneItem item)
        {
            if (activeStep != null)
                activeStep.Add(item);
            else
                m_SceneItemRegistry.RemoveItemById(item.identifiable.identifier);
        }

        public void OnSceneItemRemoved(ISceneItem item)
        {
            if (activeStep != null)
                activeStep.Remove(item);
        }

        public bool IntializeWorkflow(string previousPath, string filePath)
        {
            if (!File.Exists(filePath))
            {
                string workflowName = Path.GetFileNameWithoutExtension(filePath);
                string workflowAssetDirectory = Path.Combine(Path.GetDirectoryName(filePath), workflowName);
                if(previousPath.IsNullOrEmpty())
                {
                    m_CurrentWorkflow = workflowName;
                    if (!Directory.Exists(workflowAssetDirectory))
                        Directory.CreateDirectory(workflowAssetDirectory);
                    return true;
                }
                else if(!Directory.Exists(workflowAssetDirectory))
                {
                    m_CurrentWorkflow = workflowName;
                    string previousName = Path.GetFileNameWithoutExtension(previousPath);
                    string previousAssetDirectory = Path.Combine(Path.GetDirectoryName(previousPath),previousName);
                    Directory.Move(previousAssetDirectory, workflowAssetDirectory);
                    File.Move(previousPath, filePath);
                    workflowLoaded.Invoke();
                    return true;
                }
            }

            return false;
        }

        public void WriteWorkflow()
        {
            string workflowFilePath = Path.Combine(workflowDirectory, CurrentWorkflow + ".work");
            WorkflowData data = m_WorkflowDataFactory.Create(m_StepItems);
            data.parts = m_PartData.ToArray();
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(workflowFilePath, jsonData);
        }

        public void LoadWorkflow(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            ClearItems();

            // remove listeners here to prevent deletion of loaded items
            m_SceneItemRegistry.itemAdded.RemoveListener(OnSceneItemRegistered);
            m_SceneItemRegistry.itemRemoved.RemoveListener(OnSceneItemRemoved);

            string stringData = File.ReadAllText(filePath);
            WorkflowData data = JsonUtility.FromJson<WorkflowData>(stringData);
            m_PartData.AddRange(data.parts);

            foreach (WorkflowStepData stepData in data.steps)
            {
                IStepItem item = m_StepFactory.Create(stepData, m_StepItemPrefab);
                m_StepItems.Add(item);
                foreach (ISceneItem sceneItem in item.sceneItems)
                    sceneItem.itemTransform.gameObject.SetActive(false);
            }

            // add listeners again
            m_SceneItemRegistry.itemAdded.AddListener(OnSceneItemRegistered);
            m_SceneItemRegistry.itemRemoved.AddListener(OnSceneItemRemoved);

            m_CurrentWorkflow = Path.GetFileNameWithoutExtension(filePath);

            stepsChanged.Invoke(m_StepItems);
            workflowLoaded.Invoke();
        }

        public void ClearItems()
        {
            activeStep = null;
            while (m_StepItems.Count > 0)
            {
                IStepItem item = m_StepItems[0];
                m_StepItems.RemoveAt(0);
                item.Dispose();
            }
            m_PartData.Clear();
            stepsChanged.Invoke(m_StepItems);
        }

        private class WorkflowStepsChanged : UnityEvent<IReadOnlyList<IStepItem>>
        {
        }

        private class ActiveStepChanged : UnityEvent<int>
        {
        }
    }
}