// <copyright file=IPrefabFactory.cs/>
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

using UnityEngine;

namespace Aci.Unity.Scene
{
    /// <summary>
    ///     Interface for factory that instantiates GameObjects via prefabs associated with a string name.
    /// </summary>
    public interface IPrefabFactory
    {
        /// <summary>
        ///     Registers a prefab with the factory.
        /// </summary>
        /// <param name="prefab">Target prefab to register.</param>
        /// <remarks>
        ///     If this method is used a type name has to be chosen for the registered prefab by the factory.
        ///     This should prefarably be a value taken from the added prefab.
        /// </remarks>
        void RegisterPrefab(GameObject prefab);

        /// <summary>
        ///     Registers a prefab with the factory.
        /// </summary>
        /// <param name="prefab">Target prefab to register.</param>
        /// <param name="type">Type under which to register the prefab.</param>
        void RegisterPrefab(GameObject prefab, string type);

        /// <summary>
        ///     Creates an instance of the prefab corresponding to a given string type.
        /// </summary>
        /// <param name="type">Target registered type as string.</param>
        /// <returns>Instantiated prefab, should be null if type not known by factory.</returns>
        GameObject CreateInstance(string type);

        /// <summary>
        ///     Checks whether factory can instantiate a certain prefab type.
        /// </summary>
        /// <param name="type">Target prefab type as sstring.</param>
        /// <returns>True if type can be instantiated, false otherwise.</returns>
        bool CanCreate(string type);

        /// <summary>
        ///     Get all types that an by instiated by the factory in an array.
        /// </summary>
        /// <returns>String array of known types.</returns>
        string[] GetTypes();
    }
}