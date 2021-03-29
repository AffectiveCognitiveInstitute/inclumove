// <copyright file=UI_ChatWindow.cs/>
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

using BotConnector.Unity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface
{
    public class UI_ChatWindow : MonoBehaviour
    {
        public ScrollRect        Rect;
        public ScrollRectTweener Tweener;

        /// <summary>
        ///     Automatically adds an object to the chat window
        /// </summary>
        /// <param name="go">The GameObject or Prefab you want to attach</param>
        /// <param name="instantiate">Set to true if you want to make an (new) instance of the GameObject</param>
        /// <
        /// <param name="resetScale">set to true if the object should definitely a scale of 1,1,1</param>
        public GameObject Add(GameObject go, bool instantiate = false, bool resetScale = false)
        {
            GameObject g;
            if (instantiate)
            {
                g = Instantiate(go, Rect.content);
            }
            else
            {
                g = go;
                g.transform.parent = Rect.content;

                if (resetScale)
                    g.transform.localScale = Vector3.one;
            }

            Tweener.ScrollVertical(0);

            return g;
        }

        /// <summary>
        ///     Remove an item at a specific index position
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (Rect.content.childCount > index)
                Destroy(Rect.content.GetChild(index).gameObject);
            else
                Debug.LogWarning("ChatWindow: Warning, the child at index " + index + " does not exist");
        }
    }
}