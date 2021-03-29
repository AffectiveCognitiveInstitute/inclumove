// <copyright file=IdleChecker.cs/>
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
// <date>10/02/2018 14:54</date>

using System.Threading.Tasks;

using UnityEngine;
using Zenject;

using Aci.Unity.Events;
using Aci.Unity.Gamification;
using Aci.Unity.Util;

namespace Aci.Unity.UserInterface
{
    public class IdleChecker : MonoBehaviour
                             , IAciEventHandler<WorkflowStepEndedArgs>
                             , IAciEventHandler<WorkflowStopArgs>
                             , IAciEventHandler<UserLoginArgs>
                             , IAciEventHandler<UserLogoutArgs>
    {
        private IAciEventManager __broker;

        private bool _active;

        private readonly AsyncTimeProvider _provider = new AsyncTimeProvider();

        [Inject]
        private IUserManager _userManager;

        /// <summary>
        ///     Seconds until system classifies user as idle and initiates logout.
        /// </summary>
        public float waitTime;

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
            }
        }

        void OnDestroy()
        {
            UnregisterFromEvents();
        }

        /// <inheritdoc />
        public void OnEvent(UserLoginArgs args)
        {
            _active = true;
            _provider.Reset(true);
        }

        /// <inheritdoc />
        public void OnEvent(UserLogoutArgs args)
        {
            _active = false;
        }
        
        /// <inheritdoc />
        public void RegisterForEvents()
        {
            __broker?.AddHandler<WorkflowStepEndedArgs>(this);
            __broker?.AddHandler<WorkflowStopArgs>(this);
            __broker?.AddHandler<UserLoginArgs>(this);
            __broker?.AddHandler<UserLogoutArgs>(this);
        }

        /// <inheritdoc />
        public void UnregisterFromEvents()
        {
            __broker?.RemoveHandler<WorkflowStepEndedArgs>(this);
            __broker?.RemoveHandler<WorkflowStopArgs>(this);
            __broker?.RemoveHandler<UserLoginArgs>(this);
            __broker?.RemoveHandler<UserLogoutArgs>(this);
        }

        /// <inheritdoc />
        public void OnEvent(WorkflowStepEndedArgs args)
        {
            _provider.Reset(false);
        }

        /// <inheritdoc />
        public void OnEvent(WorkflowStopArgs args)
        {
            _active = false;
        }

        /// <summary>
        ///     Waits until timeout value is reached the causes the demo to reset by logging out the current user.
        /// </summary>
        public async void WaitForIdleDemoReset()
        {
            while (true)
            {
                await Task.Delay(1000);
                if (!_active)
                    continue;
                if (_provider.elapsed.TotalSeconds > waitTime)
                    _userManager.CurrentUser = null;
            }
        }

        public async void Start()
        {
            _provider.paused = false;
            //start async IdleReset
            WaitForIdleDemoReset();
        }
    }
}