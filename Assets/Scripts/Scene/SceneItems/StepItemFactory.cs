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

using System;
using Aci.Unity.Data.JsonModel;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Scene.SceneItems
{
    /// <summary>
    ///     Factory for SceneItem instantiation from prefabs.
    /// </summary>
    public class StepItemFactory : IFactory<WorkflowStepData, UnityEngine.Object, IStepItem>
    {
        private IInstantiator m_Instantiator;
        private SceneItem.Factory m_ItemFactory;

        [Zenject.Inject]
        private void Construct(IInstantiator instantiator, SceneItem.Factory itemFactory)
        {
            m_Instantiator = instantiator;
            m_ItemFactory = itemFactory;
        }

        public IStepItem Create(WorkflowStepData param, UnityEngine.Object prefab)
        {
            IStepItem item = m_Instantiator.InstantiatePrefab(prefab).GetComponent<IStepItem>();

            item.identifiable.identifier = param.id;
            item.duration = param.durations[0];
            item.repetitions = param.repetitions;
            item.automatic = param.automatic;
            item.mount = param.mount;
            item.unmount = param.unmount;
            item.control = param.control;
            item.partId = param.partId;
            item.gripperId = param.gripperId;
            item.name = param.name;

            byte levels = 0;
            foreach (SceneItemData itemData in param.items)
            {
                ISceneItem sceneItem = m_ItemFactory.Create(itemData);
                item.Add(sceneItem);
                levels |= sceneItem.levelable.level;
            }

            return item;
        }
    }
}