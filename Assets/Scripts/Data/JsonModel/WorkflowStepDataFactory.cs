// <copyright file=SceneItemFactory.cs/>
// <copyright>
//   Copyright (c) 2018, Affective & Cognitive Institute
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
// <date>07/12/2018 05:59</date>

using System.Collections.Generic;
using Aci.Unity.Data.JsonModel;
using Aci.Unity.Scene.SceneItems;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Data.JsonModel
{
    /// <summary>
    ///     Factory for SceneItem instantiation from prefabs.
    /// </summary>
    public class WorkflowStepDataFactory : IFactory<IStepItem, WorkflowStepData>
    {
        private SceneItemData.Factory m_Factory;

        [Zenject.Inject]
        private void Construct(SceneItemData.Factory factory)
        {
            m_Factory = factory;
        }

        public WorkflowStepData Create(IStepItem param)
        {
            WorkflowStepData data = new WorkflowStepData();

            data.id = param.identifiable.identifier.ToString("N");
            data.name = param.name;
            data.automatic = param.automatic;
            data.triggerId = param.triggerId;
            data.durations = new [] {param.duration.x, param.duration.y, param.duration.z};
            data.repetitions = param.repetitions;
            data.currentRepetitions = param.repetitions;

            byte levels = 0;
            data.items = new SceneItemData[param.sceneItems.Count];
            for (int i = 0; i < param.sceneItems.Count; ++i)
            {
                SceneItemData itemData = m_Factory.Create(param.sceneItems[i]);
                data.items[i] = itemData;
                levels |= itemData.levels;
            }
            data.levels = levels;

            return data;
        }
    }
}