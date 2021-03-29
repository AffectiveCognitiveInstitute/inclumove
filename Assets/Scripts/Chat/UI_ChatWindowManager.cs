// <copyright file=UI_ChatWindowManager.cs/>
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BotConnector.Unity.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface
{
    public class UI_ChatWindowManager : MonoBehaviour
    {
        private bool  animate           = false;
        private float animationDuration = 0;

        public UI_ChatWindow ChatWindow;
        public GameObject    Prf_CardScrollView;
        public GameObject    Prf_HeroCard;

        [Header("Prefab-References")] public GameObject Prf_Intro;

        public  GameObject Prf_TextCard;
        public  GameObject Prf_ThumbnailCard;
        public  GameObject Prf_VideoCard;
        private float      time = 0;

        private void Start()
        {
            //StartIntroAnimation(2);
        }


        /// <summary>
        ///     This is not finished yet and will only create a normal card with text
        /// </summary>
        /// <param name="duration"></param>
        public void StartIntroAnimation(float duration)
        {
            //StartCoroutine(DoIntro(duration, animate));
        }

        private IEnumerator DoIntro(float time, bool animate)
        {
            ChatWindow.Add(Prf_Intro, true).AddComponent<UI_ChatItem>().SetupCard("Hi, ich bin KoBeLU!");

            float currentTime = 0f;
            while (currentTime < time)
            {
                currentTime += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        ///     Adds a simple text-only card to the chat history
        /// </summary>
        /// <param name="text">Text Message of the card</param>
        public void AttachTextCard(string text)
        {
            ChatWindow.Add(Prf_TextCard, true).AddComponent<UI_ChatItem>().SetupCard(text);
        }

        /// <summary>
        ///     Adds a Hero Card to the Scroll View
        /// </summary>
        /// <param name="text">Text Message of the card</param>
        /// <param name="sprite">Sprite to display on card</param>
        public void AttachHeroCard(string text, Sprite sprite)
        {
            ChatWindow.Add(Prf_HeroCard, true).AddComponent<UI_ChatItem>().SetupCard(text, sprite);
        }

        /// <summary>
        ///     Adds a Thumbnail Card to the ScrollView.
        /// </summary>
        /// <param name="text">Text Message of the card</param>
        /// <param name="buttonText">Text displayed on the button</param>
        /// <param name="actionToBeCalled">Action or method that should be fired when the button is clicked</param>
        public void AttachThumbnailCard(string text, string buttonText, UnityAction actionToBeCalled)
        {
            ChatWindow.Add(Prf_ThumbnailCard, true).AddComponent<UI_ChatItem>()
                      .SetupCard(text, null, buttonText, actionToBeCalled);
        }

        /// <summary>
        ///     Adds a card showing a selection of available loadable workflows.
        /// </summary>
        public void AttachWorkflowSelectionCard(Action<string> callback)
        {
            GameObject textCard = ChatWindow.Add(Prf_TextCard, true);
            Transform contentTransform = textCard.GetComponent<ActivityRenderer>().TextPanel.transform;
            contentTransform.GetComponent<TextMeshProUGUI>().text =
                "Ich habe eine Auswahl von Übungen für dich. Such dir eine aus! Oder sollen wir weitermachen wo du aufgehört hast?";
            GameObject scrollView = Instantiate(Prf_CardScrollView);
            scrollView.transform.SetParent(contentTransform.parent);
            scrollView.transform.localScale = contentTransform.parent.localScale;

            List<string> workflows = getWorkflows();

            foreach (string str in workflows)
            {
                //manually fetch data from workflow file
                string name = "", description = "", category = "", creator = "";
                byte[] thumbnail = null;
                using (StreamReader reader = File.OpenText(str))
                {
                    JObject jObj = (JObject) JToken.ReadFrom(new JsonTextReader(reader));
                    // do stuff
                    name = (string) jObj["m_Name"];
                    description = (string) jObj["m_Description"];
                    category = (string) jObj["Category"];
                    creator = (string) jObj["Creator"];
                    if (jObj["Thumbnail"].ToObject<object>() != null)
                        thumbnail = (byte[]) jObj["Thumbnail"]["$value"];
                }

                //construct card
                GameObject heroCard = Instantiate(Prf_HeroCard);
                HeroCardRenderer cardInfo = heroCard.GetComponent<HeroCardRenderer>();
                Button button = heroCard.AddComponent<Button>();
                //if we got a valid workflow name set it, otherwise display the filename
                cardInfo.Title.GetComponent<Text>().text = name != null ? name : Path.GetFileName(str);
                cardInfo.Subtitle.GetComponent<Text>().text = category;
                cardInfo.Text.GetComponent<Text>().text = description;
                //if we got thumbnail data try to build sprite
                if (thumbnail != null)
                {
                    Texture2D tex = new Texture2D(64, 64, TextureFormat.BGRA32, false);
                    tex.LoadRawTextureData(thumbnail);
                    //tex.LoadImage(thumbnail);
                    tex.Apply();
                    Sprite workflowSprite = Sprite.Create(tex, new Rect(0, 64, 64, -64), new Vector2(0, 0));
                    cardInfo.HeroImage.sprite = workflowSprite;
                }

                button.targetGraphic = cardInfo.HeroImage;
                button.onClick.AddListener(() => { callback(Path.GetFileName(str)); });
                heroCard.transform.SetParent(scrollView.GetComponent<ScrollRect>().content);
                heroCard.transform.localScale = heroCard.transform.parent.localScale;
            }
        }

        private List<string> getWorkflows()
        {
            string workflowDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                 Path.DirectorySeparatorChar + "kbl_workflows";
            List<string> fileNames = new List<string>();
            try
            {
                fileNames = new List<string>(Directory.GetFiles(workflowDir));

                /*for (int i = 0; i < fileNames.Count; i++)
                    fileNames[i] = Path.GetFileName(fileNames[i]);*/
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Debug.LogError("UI_ChatWindowManager: Failed to create WorkflowCard, ERROR: " + UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Debug.LogError("UI_ChatWindowManager: Failed to create WorkflowCard, ERROR: " + PathEx.Message);
            }
            catch (DirectoryNotFoundException DirNfEx)
            {
                Debug.LogError("UI_ChatWindowManager: Failed to create WorkflowCard, ERROR: " + DirNfEx.Message);
            }
            catch (ArgumentException aEX)
            {
                Debug.LogError("UI_ChatWindowManager: Failed to create WorkflowCard, ERROR: " + aEX.Message);
            }

            return fileNames;
        }
    }
}