// <copyright file=multiDisplayInput.cs/>
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

using Aci.Unity.UserInterface;
using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.Tests
{
    public class multiDisplayInput : MonoBehaviour
    {
        public Text           DebugText;
        public DisplayManager man;

        // Update is called once per frame
        private void Update()
        {
            if (DebugText == null)
                return;
            bool multiDisplay = man.HasMultipleDisplays();

            /*var mouseOverWindow = EditorWindow.mouseOverWindow;
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            Type type = assembly.GetType("UnityEditor.GameView");


            if (!type.IsInstanceOfType(mouseOverWindow))
                return;*/

            int displayID = 0;
            //var displayField = type.GetField("m_TargetDisplay", BindingFlags.NonPublic | BindingFlags.Instance);
            //displayID = (int)displayField.GetValue(mouseOverWindow);

            Vector3 pos = Input.mousePosition;

            DebugText.text = "MultiDisplay: " + multiDisplay + " | Pos.: " + pos + " | Display: " + displayID;
        }
    }
}