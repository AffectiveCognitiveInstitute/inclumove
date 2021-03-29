// <copyright file=TrackerToggle.cs/>
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
// <date>07/12/2018 15:53</date>

using Aci.Unity.Events;
using Aci.Unity.Gamification;
using Aci.Unity.Scene;
using Aci.Unity.UI.Tweening;
using Aci.Unity.Util;
using UnityEngine;
using Zenject;

namespace Aci.Unity.UserInterface
{
    /// <summary>
    ///     Switches between pyramid and timeline implementation of the gamification tracker.
    /// </summary>
    public class TrackerToggle : MonoBehaviour
                               , IAciEventHandler<WorkflowLoadArgs>
                               , IAciEventHandler<DemoResetArgs>
    {
        private IAciEventManager __broker;

        public GamificationPyramid  pyramid  = null;
        public GamificationTimeline timeline = null;

        private AlphaTweener trackerGroupTweener = null;
        private TweenerDirector timelineDirector = null;
        private TweenerDirector pyramidDirector = null;

        [SerializeField] private bool usePyramid;

        [SerializeField] private bool useTimeline;

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

        /// <inheritdoc />
        public void RegisterForEvents()
        {
            broker?.AddHandler<WorkflowLoadArgs>(this);
            broker?.AddHandler<DemoResetArgs>(this);
        }

        /// <inheritdoc />
        public void UnregisterFromEvents()
        {
            broker?.RemoveHandler<WorkflowLoadArgs>(this);
            broker?.RemoveHandler<DemoResetArgs>(this);
        }

        /// <inheritdoc />
        public async void OnEvent(WorkflowLoadArgs args)
        {
            UpdateTrackerUsage(usePyramid, useTimeline);
            if (usePyramid)
            {
                await trackerGroupTweener.PlayForwardsAsync();
                pyramidDirector?.PlayForwards();
            }
            else if (useTimeline)
            {
                await trackerGroupTweener.PlayForwardsAsync();
                timelineDirector?.PlayForwards();
            }
        }

        /// <inheritdoc />
        public async void OnEvent(DemoResetArgs args)
        {
            if (usePyramid)
            {
                await trackerGroupTweener.PlayReverseAsync();
                pyramidDirector?.PlayReverse();
            }
            else if (useTimeline)
            {
                await trackerGroupTweener.PlayReverseAsync();
                timelineDirector?.PlayReverse();
            }
        }

        private void Start()
        {
            trackerGroupTweener = GetComponent<AlphaTweener>();
            timelineDirector = timeline?.GetComponent<TweenerDirector>();
            pyramidDirector = pyramid?.GetComponent<TweenerDirector>();
            UpdateTrackerUsage(usePyramid, useTimeline);
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        /// <summary>
        ///     Updates the current Tracker toggle state.
        /// </summary>
        /// <param name="usePyr">True if pyramid should be used, False otherwise.</param>
        /// <param name="useLine">True if timeline should be used, False otherwise.</param>
        public void UpdateTrackerUsage(bool usePyr, bool useLine)
        {
            if (usePyr && useLine)
                return;

            if (usePyr)
            {
                useTimeline = false;
                usePyramid = true;
                pyramid.SetActive(true);
                //timeline.SetActive(false);
            }
            else if (useLine)
            {
                useTimeline = true;
                usePyramid = false;
                pyramid.SetActive(false);
                //timeline.SetActive(true);
            }
            else
            {
                useTimeline = false;
                usePyramid = false;
                pyramid.SetActive(false);
                //timeline.SetActive(false);
            }
        }
    }
}