// <copyright file=UserAvatar.cs/>
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

using System.IO;
using Aci.Unity.Events;
using Aci.Unity.Gamification;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Aci.Unity.UserInterface
{
    public class UserAvatar : MonoBehaviour
                            , IAciEventHandler<UserLoginArgs>
                            , IAciEventHandler<UserLogoutArgs>
    {
        private IAciEventManager __broker;
        public  Image          avatarImage;

        public Sprite fallbackImage;

        [Inject] private IUserManager userManager;

        [Inject]
        private IAciEventManager broker
        {
            get { return __broker; }
            set
            {
                if (value == null)
                    return;
                UnregisterFromEvents();
                __broker = value;
                RegisterForEvents();
                // this is a workaround to on destroy not being called
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        }

        /// <inheritdoc />
        public void RegisterForEvents()
        {
            broker?.AddHandler<UserLoginArgs>(this);
            broker?.AddHandler<UserLogoutArgs>(this);
        }

        /// <inheritdoc />
        public void UnregisterFromEvents()
        {
            broker?.RemoveHandler<UserLoginArgs>(this);
            broker?.RemoveHandler<UserLogoutArgs>(this);
        }

        /// <inheritdoc />
        public void OnEvent(UserLoginArgs args)
        {
            Toggle(true);
        }

        /// <inheritdoc />
        public void OnEvent(UserLogoutArgs args)
        {
            Toggle(false);
        }

        public void Toggle(bool active)
        {
            if (!gameObject.GetComponent<CanvasGroup>().interactable && userManager?.CurrentUser != null)
            {
                gameObject.GetComponent<CanvasGroup>().interactable = false;
                avatarImage.sprite = userManager.CurrentUser.userPicture;
                return;
            }

            if (gameObject.GetComponent<CanvasGroup>().interactable)
                gameObject.GetComponent<CanvasGroup>().interactable = true;
        }

        private void OnDestroy()
        {
            broker?.RemoveHandler<UserLoginArgs>(this);
            broker?.RemoveHandler<UserLogoutArgs>(this);
        }
    }
}