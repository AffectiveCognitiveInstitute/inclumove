// <copyright file=Timeline.cs/>
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

using System.Collections.Generic;
using Aci.Unity.Events;
using Aci.Unity.Gamification;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Zenject;

namespace Aci.Unity.UserInterface
{
    /// <summary>
    ///     TimeLine UI. Displays a timeline with subdivisions and a user and ghost indicator.
    /// </summary>
    public class Timeline : UIBehaviour
                          , IAciEventHandler<UserLoginArgs>
    {
        private IAciEventManager __broker;

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

        private IUserManager manager;

        private Sprite avatarBuffer;
        private int   numSubdivs;
        private float lastPercentage;
        private int lastLine = -1;

        private Vector2 curLeftAnchor = Vector2.zero;
        private Vector2 curRightAnchor = Vector2.zero;

        [Tooltip("Prefab for a single timeline subdivision.")]
        public GameObject timelineSubPrefab;

        [Tooltip("Transform holding the timeline subdivisions.")]
        public RectTransform lineAnchor;
        [Tooltip("List of current timeline subdivisions.")]
        public List<TimelineSub> subdivisions = new List<TimelineSub>();

        [Tooltip("LineRenderer for the progress line.")]
        public UILineRenderer progressLineRenderer;
        [Tooltip("Pin for the avatar.")]
        public TimelinePin avatarPin;
        [Tooltip("Pin for the ghost")]
        public TimelinePin ghostPin;
        [Tooltip("Left anchor of the timeline.")]
        public Image leftAnchor;
        [Tooltip("Right anchor of the timeline.")]
        public Image rightAnchor;

        public int lineThickness = 3;
        public int lineSpacing = 3;

        [Tooltip("Color of the GhostPin background.")]
        public Color ghostColor;
        [Tooltip("Color of a subdivisions line if over time limit.")]
        public Color negativeColor;
        [Tooltip("Color of a subdivisions line if not yet started progress.")]
        public Color neutralColor;
        [Tooltip("Color of a subdivisions line in time limit.")]
        public Color progressColor;

        /// <inheritdoc/>
        public void RegisterForEvents()
        {
            __broker?.AddHandler<UserLoginArgs>(this);
        }

        /// <inheritdoc/>
        public void UnregisterFromEvents()
        {
            __broker?.RemoveHandler<UserLoginArgs>(this);
        }

        [Inject]
        public void Construct(IAciEventManager eventBroker, IUserManager userManager)
        {
            broker = eventBroker;
            manager = userManager;
        }

        /// <inheritdoc/>
        public void OnEvent(UserLoginArgs args)
        {
            Sprite avatarSprite = manager.CurrentUser?.userPicture;
            if (avatarSprite == null)
                return;
            avatarPin.pinHead.sprite = avatarSprite;
        }

        private void Awake()
        {
            avatarBuffer = avatarPin.pinHeadBackground.sprite;
        }

        private void OnDestroy()
        {
            UnregisterFromEvents();
        }

        public void Clear()
        {
            for (int i = 0; i < subdivisions.Count; i++)
            {
                subdivisions[i].transform.parent = null;
                Destroy(subdivisions[i].gameObject);
            }
            subdivisions.Clear();
        }

        public void SetSubdivisions(int divisions, float[] percentages)
        {
            calculateAndSetSubdivisions(divisions, percentages);
        }

        private void calculateAndSetSubdivisions(int divisions, float[] percentages)
        {
            float maxWidth = lineAnchor.rect.size.x;
            lineThickness = lineSpacing = Mathf.FloorToInt(lineAnchor.rect.height);

            lastLine = -1;
            rightAnchor.color = neutralColor;
            progressLineRenderer.LineThickness = lineThickness;

            Clear();

            numSubdivs = divisions;
            //get the amount of zero time steps
            int nZeroSteps = 0;
            for (int i = 0; i < percentages.Length; ++i)
            {
                if (percentages[i] == 0)
                    ++nZeroSteps;
            }

            Vector2 currentPosition = Vector2.zero;
            Vector2 spacing = new Vector2(lineSpacing*2, 0);

            // we subtract one spacer at the end + one end step + it's spacer
            maxWidth -= (numSubdivs-2) * spacing.x + (nZeroSteps - 1) * spacing.x;

            TimelineSub item = null;

            for (int i = 0; i < numSubdivs -1; i++)
            {
                float subWidth = percentages[i] == 0 ? spacing.x : (maxWidth * percentages[i]);
                item = Instantiate(timelineSubPrefab).GetComponent<TimelineSub>();
                item.lineRenderer.LineThickness = lineThickness;
                subdivisions.Add(item);
                item.transform.SetParent(lineAnchor, false);

                currentPosition = item.Setup(subWidth, currentPosition) + spacing;
            }
            // add last step manually since it should note be visible & should have no spacing
            item = Instantiate(timelineSubPrefab).GetComponent<TimelineSub>();
            item.lineRenderer.LineThickness = lineThickness;
            subdivisions.Add(item);
            item.transform.SetParent(lineAnchor, false);
            currentPosition = item.Setup(1, currentPosition-spacing);
        }

        /// <summary>
        /// Sets the active subline.
        /// </summary>
        /// <param name="subline">Target subline.</param>
        public void SetActiveLine(int subline)
        {
            //update last step to fully colored
            if(lastLine > -1)
                subdivisions[lastLine].SetColor(progressColor);

            Debug.Log(subdivisions[subline].lineRenderer.Points.Length);
            //set new line anchors
            curLeftAnchor = subdivisions[subline].lineRenderer.Points[0];
            curRightAnchor = subdivisions[subline].lineRenderer.Points[subdivisions[subline].lineRenderer.Points.Length - 1];

            //set avatar position
            Vector2 avatarPosition = curLeftAnchor;
            avatarPosition -= new Vector2(lineAnchor.rect.xMin, 0);

            progressLineRenderer.Points[0] = curLeftAnchor;
            progressLineRenderer.Points[1] = curLeftAnchor;

            if (subline == numSubdivs - 1)
                rightAnchor.color = progressColor;

            lastLine = subline;
        }

        /// <summary>
        ///     Sets the avatar position on the target subline and updates avatar look.
        /// </summary>
        /// <param name="percentage">current percentage</param>
        /// <param name="accumulatedPercentage">Percentage for this and previous working steps.</param>
        /// <param name="ghostPercentage">Percentage for ghost.</param>
        public void SetAvatarPosition(float percentage, float accumulatedPercentage, float ghostPercentage)
        {
            if (subdivisions == null || subdivisions.Count == 0) return;

            // get current global avatar position
            Vector2 avatarPosition = Vector2.Lerp(curLeftAnchor,
                                                  curRightAnchor,
                                                  percentage);
            avatarPosition -= new Vector2(lineAnchor.rect.xMin, 0);

            // set avatar position
            avatarPin.SetPosition(avatarPosition);

            bool behindGhost = IsUserBehindGhost(accumulatedPercentage, ghostPercentage);
            // set avatar-pinhead color
            avatarPin.SetHeadColor(behindGhost ? progressColor : ghostColor);
            //set avatar background
            avatarPin.pinHeadBackground.sprite = behindGhost ? avatarBuffer : ghostPin.pinHeadBackground.sprite;
            avatarPin.pinHeadMask.sprite = behindGhost ? avatarBuffer : ghostPin.pinHeadBackground.sprite;
        }

        /// <summary>
        /// Sets ghost performance measured by entire workflow.
        /// </summary>
        /// <param name="ghostPercentage">Current ghost percentage.</param>
        /// <param name="avatarPercentage">Current accumulated vatar percentage.</param>
        public void SetGhostPerformance(float ghostPercentage, float avatarPercentage)
        {
            if (subdivisions == null || subdivisions.Count == 0) return;

            bool behind = IsUserBehindGhost(avatarPercentage, ghostPercentage);
            if (!behind || ghostPercentage >= 1 && behind)
            {
                ghostPin.SetHeadColor(new Color(1, 1, 1, 0));
                return;
            }

            Vector2 ghostPosition = Vector2.zero;
       
            // get ghost position on timeline
            ghostPosition = Vector2.Lerp(Vector2.zero, new Vector2(lineAnchor.rect.size.x, 0), ghostPercentage);

            // set pin position
            ghostPin.SetPosition(ghostPosition);

            // set ghost visibility
            ghostPin.SetHeadColor(Color.Lerp(ghostColor - new Color(0, 0, 0, 1)
                                           , ghostColor
                                           , Mathf.Clamp((avatarPin.GetPosition() - ghostPin.GetPosition()).magnitude, 0, 50) / 50.0f));
        }

        public void UpdateProgress(float percentage)
        {
            if (subdivisions == null || subdivisions.Count == 0) return;
            lastPercentage = percentage;

            // if line not filled draw progress line
            if (percentage < 1.0f)
            {
                progressLineRenderer.Points[0] = curLeftAnchor;
                progressLineRenderer.Points[1] = Vector2.Lerp(curLeftAnchor
                                                            , curRightAnchor
                                                            , percentage);
                progressLineRenderer.SetVerticesDirty();
                progressLineRenderer.color = progressColor;
                return;
            }

            //set progress line transparent
            progressLineRenderer.color = new Color(0, 0, 0, 0);
            // set line color according to max overflow
            subdivisions[lastLine].SetColor(Color.Lerp(progressColor, negativeColor, (percentage - 1) * 4));
            // transform line
            subdivisions[lastLine].Deform(percentage, 16.0f);
        }

        private bool IsUserBehindGhost(float avatarPercentage, float ghostPercentage)
        {
            return avatarPercentage < ghostPercentage;
        }
    }
}