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

using Aci.Unity.Scene.SceneItems;
using Zenject;

namespace Aci.Unity.Data.JsonModel
{
    /// <summary>
    ///     Factory for SceneItem instantiation from prefabs.
    /// </summary>
    public class SceneItemDataFactory : IFactory<ISceneItem, SceneItemData>
    {
        public SceneItemData Create(ISceneItem param)
        {
            SceneItemData data = new SceneItemData();

            data.id = param.identifiable.identifier;
            data.position = param.itemTransform.position;
            data.rotation = param.itemTransform.rotation.eulerAngles;
            data.size = param.scalable.size;
            data.color = param.colorable.color;
            data.payloadType = param.payloadViewController.payloadType;
            data.payload = param.payloadViewController.payload;
            data.levels = param.levelable.level;
            data.delay = param.payloadViewController.delay;

            return data;
        }
    }
}