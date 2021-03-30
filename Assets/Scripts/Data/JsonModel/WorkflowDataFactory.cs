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
using Aci.Unity.Scene.SceneItems;
using Zenject;

namespace Aci.Unity.Data.JsonModel
{
    /// <summary>
    ///     Factory for SceneItem instantiation from prefabs.
    /// </summary>
    public class WorkflowDataFactory : IFactory<List<IStepItem>, WorkflowData>
    {
        private WorkflowStepData.Factory m_Factory;

        [Zenject.Inject]
        private void Construct(WorkflowStepData.Factory factory)
        {
            m_Factory = factory;
        }

        public WorkflowData Create(List<IStepItem> param)
        {
            WorkflowData data = new WorkflowData();
            data.steps = new WorkflowStepData[param.Count];
            int totalSteps = 0;
            for(int i = 0; i < param.Count; ++i)
            {
                WorkflowStepData stepData = m_Factory.Create(param[i]);
                data.steps[i] = stepData;
                totalSteps += stepData.repetitions + 1;
            }
            data.numTotalSteps = totalSteps;
            data.name = "";
            data.orderId = 0;
            data.referenceMarks = new Models.Position[]
            {
                new Models.Position() { X = 0, Y = 0 },
                new Models.Position() { X = 0, Y = 0 }
            };
            return data;
        }
    }
}