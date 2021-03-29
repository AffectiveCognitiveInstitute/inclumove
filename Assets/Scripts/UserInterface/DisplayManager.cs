// <copyright file=DisplayManager.cs/>
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
using UnityEngine.SceneManagement;
using Aci.Unity.Logging;

namespace Aci.Unity.UserInterface
{
    /// <summary>
    ///     Provides interface for activating additional displays.
    /// </summary>
    public class DisplayManager : MonoBehaviour
    {
        private bool dirty = true;

        public  bool DontUseMultiDisplay = false;
        private int  projDisplay         = 1;
        private int  uiDisplay;

        public int UIDisplay
        {
            get { return uiDisplay; }
            set
            {
                if (uiDisplay == value)
                    return;
                uiDisplay = value;
                dirty = true;
            }
        }

        public int ProjectionDisplay
        {
            get { return projDisplay; }
            set
            {
                if (projDisplay == value)
                    return;
                projDisplay = value;
                dirty = true;
            }
        }

        private void Awake()
        {
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        private void SceneManagerOnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
        {
            ReassignDisplays();
        }

        public void ReassignDisplays()
        {
            //if display are not dirty or there are not enough displays or cameras
            if (DontUseMultiDisplay || !dirty || Display.displays.Length == 1 || Camera.allCamerasCount == 1)
                return;

            //set displays to display and activate
            Camera.main.targetDisplay = UIDisplay;
            foreach (Camera cam in Camera.allCameras)
            {
                if (Camera.main == cam)
                    continue;
                cam.targetDisplay = ProjectionDisplay;
                break;
            }

            Display.displays[uiDisplay].Activate();
            Display.displays[ProjectionDisplay].Activate();
            AciLog.Log("DisplayManager", $"Displaying UI on display {UIDisplay} and Projection on display {ProjectionDisplay}.");
        }

        public bool HasMultipleDisplays()
        {
            return !DontUseMultiDisplay && (Application.isEditor || Display.displays.Length > 1);
        }
    }
}