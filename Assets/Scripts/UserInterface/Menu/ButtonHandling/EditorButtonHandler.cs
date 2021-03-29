// <copyright file=MuteButtonHandler.cs/>
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
// <date>07/17/2018 17:11</date>

using UnityEngine;
using Zenject;

using Aci.Unity.Audio;
using Aci.Unity.Events;
using Aci.Unity.UI;
using Aci.Unity.Util;
using Aci.Unity.Gamification;

namespace Aci.Unity.UserInterface.Menu.ButtonHandling
{
    public class EditorButtonHandler : MonoBehaviour
    {
        [SerializeField]
        private string m_SceneToUnload;

        [SerializeField]
        private string m_SceneToLoad;

        private ISceneService m_SceneService;
        private IUserManager m_UserManager;

        [Inject]
        private void Construct(ISceneService sceneService, IUserManager userManager)
        {
            m_SceneService = sceneService;
            m_UserManager = userManager;
        }

        private void Start()
        {
           gameObject.SetActive(m_UserManager.CurrentUser?.isAdmin ?? false);
        }

        public void Toggle()
        {
            m_SceneService.SwitchScene(m_SceneToUnload, m_SceneToLoad);
        }
    }
}