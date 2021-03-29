/// Credit BinaryX 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1945602
/// Updated by ddreaper - removed dependency on a custom ScrollRect script. Now implements drag interfaces and standard Scroll Rect.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// Enables snapping to items of a <see cref="ScrollRect"/> with buttons and pagination.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class HorizontalScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private Transform screensContainer;

        private int screens = 1;
        private int startingScreen = 1;

        private bool fastSwipeTimer = false;
        private bool isFastSwipe = false;
        private int fastSwipeCounter = 0;
        private int fastSwipeTarget = 30;

        private List<Vector3> positions;
        private ScrollRect scroll_rect;
        private Vector3 lerp_target;
        private bool lerp;

        private int containerSize;

        private bool startDrag = true;
        private Vector3 startPosition = new Vector3();
        private int currentScreen;

        [Tooltip("The gameobject that contains toggles which suggest pagination. (optional)")]
        public GameObject Pagination;
        [Tooltip("Button to go to the next page. (optional)")]
        public GameObject NextButton;
        [Tooltip("Button to go to the previous page. (optional)")]
        public GameObject PrevButton;

        public bool UseFastSwipe = true;
        public int FastSwipeThreshold = 100;

        private void Start()
        {
            scroll_rect = gameObject.GetComponent<ScrollRect>();
            screensContainer = scroll_rect.content;
            DistributePages();

            screens = screensContainer.childCount;

            lerp = false;

            positions = new List<Vector3>();

            if (screens > 0)
            {
                for (int i = 0; i < screens; ++i)
                {
                    scroll_rect.horizontalNormalizedPosition = (float)i / (float)(screens - 1);
                    positions.Add(screensContainer.localPosition);
                }
            }

            scroll_rect.horizontalNormalizedPosition = (float)(startingScreen - 1) / (float)(screens - 1);

            containerSize = (int)screensContainer.gameObject.GetComponent<RectTransform>().offsetMax.x;

            ChangeBulletsInfo(CurrentScreen());

            if (NextButton)
                NextButton.GetComponent<Button>().onClick.AddListener(() => { NextScreen(); });

            if (PrevButton)
                PrevButton.GetComponent<Button>().onClick.AddListener(() => { PreviousScreen(); });
        }

        private void Update()
        {
            if (lerp)
            {
                screensContainer.localPosition = Vector3.Lerp(screensContainer.localPosition, lerp_target, 7.5f * Time.deltaTime);
                if (Vector3.Distance(screensContainer.localPosition, lerp_target) < 0.005f)
                {
                    lerp = false;
                }

                //change the info bullets at the bottom of the screen. Just for visual effect
                if (Vector3.Distance(screensContainer.localPosition, lerp_target) < 10f)
                {
                    ChangeBulletsInfo(CurrentScreen());
                }
            }

            if (fastSwipeTimer)
            {
                fastSwipeCounter++;
            }

        }


        /// <summary>
        /// Snaps to the next item in the container.
        /// </summary>
        public void NextScreen()
        {
            if (CurrentScreen() < screens - 1)
            {
                lerp = true;
                lerp_target = positions[CurrentScreen() + 1];

                ChangeBulletsInfo(CurrentScreen() + 1);
            }
        }

        /// <summary>
        /// Snaps to the previous item in the container.
        /// </summary>
        public void PreviousScreen()
        {
            if (CurrentScreen() > 0)
            {
                lerp = true;
                lerp_target = positions[CurrentScreen() - 1];

                ChangeBulletsInfo(CurrentScreen() - 1);
            }
        }

        //Because the CurrentScreen function is not so reliable, these are the functions used for swipes
        private void NextScreenCommand()
        {
            if (currentScreen < screens - 1)
            {
                lerp = true;
                lerp_target = positions[currentScreen + 1];

                ChangeBulletsInfo(currentScreen + 1);
            }
        }

        //Because the CurrentScreen function is not so reliable, these are the functions used for swipes
        private void PrevScreenCommand()
        {
            if (currentScreen > 0)
            {
                lerp = true;
                lerp_target = positions[currentScreen - 1];

                ChangeBulletsInfo(currentScreen - 1);
            }
        }


        //find the closest registered point to the releasing point
        private Vector3 FindClosestFrom(Vector3 start, System.Collections.Generic.List<Vector3> positions)
        {
            Vector3 closest = Vector3.zero;
            float distance = Mathf.Infinity;

            foreach (Vector3 position in this.positions)
            {
                if (Vector3.Distance(start, position) < distance)
                {
                    distance = Vector3.Distance(start, position);
                    closest = position;
                }
            }

            return closest;
        }


        //returns the current screen that the is seeing
        public int CurrentScreen()
        {
            float visWidth = scroll_rect.viewport.GetComponent<RectTransform>().rect.width;
            float offset = screensContainer.GetComponent<HorizontalLayoutGroup>().spacing*screens + screensContainer.GetComponent<HorizontalLayoutGroup>().padding.horizontal;
            float elementWidth = (screensContainer.GetComponent<RectTransform>().rect.width-offset) / screens;

            float posX = screensContainer.gameObject.GetComponent<RectTransform>().offsetMin.x*-1 - offset;

            return (int)Math.Round(posX / elementWidth);
            /*//we should not need Math.Abs here since we scroll to a max of 0
            float absPos = screensContainer.gameObject.GetComponent<RectTransform>().offsetMin.x * -1.0f;

            //absPos = Mathf.Clamp(absPos, 1, containerSize - 1);

            float calc = (absPos / visWidth) * screens;
            Debug.Log(calc);
            //we choose the nearest since we could be only looking at some of the screen
            return (int)Math.Round(calc);*/
        }

        //changes the bullets on the bottom of the page - pagination
        private void ChangeBulletsInfo(int currentScreen)
        {
            if (Pagination)
                for (int i = 0; i < Pagination.transform.childCount; i++)
                {
                    Pagination.transform.GetChild(i).GetComponent<Toggle>().isOn = (currentScreen == i)
                        ? true
                        : false;
                }
        }

        //used for changing between screen resolutions
        private void DistributePages()
        {
            int _offset = 0;
            int _step = Screen.width;
            int _dimension = 0;

            int currentXPosition = 0;

            for (int i = 0; i < screensContainer.transform.childCount; i++)
            {
                RectTransform child = screensContainer.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
                currentXPosition = _offset + i * _step;
                child.anchoredPosition = new Vector2(currentXPosition, 0f);
                child.sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().sizeDelta.y);
            }

            _dimension = currentXPosition + _offset * -1;

            screensContainer.GetComponent<RectTransform>().offsetMax = new Vector2(_dimension, 0f);
        }

        #region Interfaces
        public void OnBeginDrag(PointerEventData eventData)
        {
            startPosition = screensContainer.localPosition;
            fastSwipeCounter = 0;
            fastSwipeTimer = true;
            currentScreen = CurrentScreen();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            startDrag = true;
            if (scroll_rect.horizontal)
            {
                if (UseFastSwipe)
                {
                    isFastSwipe = false;
                    fastSwipeTimer = false;
                    if (fastSwipeCounter <= fastSwipeTarget)
                    {
                        if (Math.Abs(startPosition.x - screensContainer.localPosition.x) > FastSwipeThreshold)
                        {
                            isFastSwipe = true;
                        }
                    }
                    if (isFastSwipe)
                    {
                        if (startPosition.x - screensContainer.localPosition.x > 0)
                        {
                            NextScreenCommand();
                        }
                        else
                        {
                            PrevScreenCommand();
                        }
                    }
                    else
                    {
                        lerp = true;
                        lerp_target = FindClosestFrom(screensContainer.localPosition, positions);
                    }
                }
                else
                {
                    lerp = true;
                    lerp_target = FindClosestFrom(screensContainer.localPosition, positions);
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            lerp = false;
            if (startDrag)
            {
                OnBeginDrag(eventData);
                startDrag = false;
            }
        }
        #endregion
    }
}