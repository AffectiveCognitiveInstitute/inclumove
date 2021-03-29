// <copyright file=TableScreensaverViewController.cs/>
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
//   James Gay, Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>08/24/2018 13:28</date>

using UnityEngine;
using UnityEngine.Video;

using Zenject;

using Aci.Unity.Events;
using Aci.Unity.UI.Tweening;

namespace Aci.Unity.UserInterface.ViewControllers
{
    [RequireComponent(typeof(VideoPlayer))]
    public class TableScreensaverViewController : MonoBehaviour
                                                , IAciEventHandler<WorkflowStopArgs>
                                                , IAciEventHandler<UserLoginArgs>
                                                , IAciEventHandler<DemoResetArgs>

    {
        [SerializeField, Tooltip("Video clip that is played when application is idling")]
        private VideoClip m_IdleClip;

        [SerializeField, Tooltip("Video clip that is played when the result clip is displayed")]
        private VideoClip m_ResultClip;

        [Inject]
        private IAciEventManager m_AciEventBroker;

        private VideoPlayer m_VideoPlayer;

        private Tweener videoTweener;

        private void Awake()
        {
            videoTweener = GetComponent<Tweener>();
            m_VideoPlayer = GetComponent<VideoPlayer>();
            OnEvent(new DemoResetArgs());
        }

        private void OnEnable()
        {
            RegisterForEvents();
        }

        private void OnDisable()
        {
            UnregisterFromEvents();
        }

        /// <inheritdoc/>
        public void RegisterForEvents()
        {
            m_AciEventBroker?.AddHandler<WorkflowStopArgs>(this);
            m_AciEventBroker?.AddHandler<UserLoginArgs>(this);
            m_AciEventBroker?.AddHandler<DemoResetArgs>(this);
        }

        /// <inheritdoc/>
        public void UnregisterFromEvents()
        {
            m_AciEventBroker?.RemoveHandler<WorkflowStopArgs>(this);
            m_AciEventBroker?.RemoveHandler<UserLoginArgs>(this);
            m_AciEventBroker?.RemoveHandler<DemoResetArgs>(this);
        }

        /// <inheritdoc/>
        public void OnEvent(UserLoginArgs args)
        {
            m_VideoPlayer.Stop();
            videoTweener.PlayReverse();
        }

        /// <inheritdoc/>
        public void OnEvent(WorkflowStopArgs args)
        {
            m_VideoPlayer.clip = m_ResultClip;
            m_VideoPlayer.isLooping = true;
            m_VideoPlayer.Play();
            if (videoTweener.elapsedTime > 0)
                return;
            videoTweener.PlayForwards();
        }

        /// <inheritdoc/>
        public void OnEvent(DemoResetArgs args)
        {
            m_VideoPlayer.clip = m_IdleClip;
            m_VideoPlayer.isLooping = true;
            m_VideoPlayer.Play();
            if (videoTweener.elapsedTime > 0)
                return;
            videoTweener.PlayForwards();
        }
    }
}