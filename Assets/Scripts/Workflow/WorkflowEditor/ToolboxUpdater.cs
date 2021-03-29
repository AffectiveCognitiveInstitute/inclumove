// <copyright file=ToolboxUpdater.cs/>
// <copyright>
//   Copyright (c) 2019, Affective & Cognitive Institute
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
// <date>10/24/2019 09:20</date>

using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aci.Unity.Scene;
using Aci.Unity.Scene.SceneItems;
using Aci.Unity.Util;
using UnityEngine;
using UnityEngine.UI.Extensions;
using WebSocketSharp;
using Zenject;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    [RequireComponent(typeof(ReorderableList))]
    public class ToolboxUpdater : MonoBehaviour
    {
        private IConfigProvider                        m_ConfigProvider;
        private SceneItemElementViewController.Factory m_ItemElementFactory;
        private ReorderableList                        m_List;
        private SceneItemTemplateDataStorage           m_Storage;
        public  GameObject                             sceneItemElementPrefab;

        [ConfigValue("assetsUrl")]
        public string assetsUrl { get; set; } = "";

        [Inject]
        private void Construct(SceneItemElementViewController.Factory itemElementFactory,
                               SceneItemTemplateDataStorage           templateDataStorage,
                               IConfigProvider                        configProvider)
        {
            m_ItemElementFactory = itemElementFactory;
            m_Storage = templateDataStorage;
            m_ConfigProvider = configProvider;
            m_List = GetComponent<ReorderableList>();

            m_ConfigProvider?.RegisterClient(this);
            //write default values to config if no config values were loaded
            if (assetsUrl.IsNullOrEmpty())
                m_ConfigProvider?.ClientDirty(this);

            Refresh();
        }

        private void OnDestroy()
        {
            m_ConfigProvider?.UnregisterClient(this);
        }

        public async void Refresh()
        {
            for (int i = 0; i < m_List.Content.childCount; ++i) Destroy(m_List.Content.GetChild(i));
            m_List.Content.DetachChildren();

            foreach (SceneItemTemplateData data in m_Storage.data)
                if (data.payloadType == PayloadType.Primitive ||
                    data.payloadType == PayloadType.Text ||
                    data.payloadType == PayloadType.ChatBot)
                {
                    SceneItemElementViewController controller =
                        m_ItemElementFactory.Create(data, data.itemName, sceneItemElementPrefab);
                    controller?.transform.SetParent(m_List.Content, false);
                }
                else
                {
                    CreateAllAtUrl(data, assetsUrl);
                }

            await Task.Delay(20);

            for (int i = 0; i < m_List.Content.childCount; ++i)
                m_List.Content.GetChild(i).GetComponent<ReorderableListElement>().isDroppableInSpace = true;
        }

        private void CreateAllAtUrl(SceneItemTemplateData data, string url)
        {
            string pattern = "";
            switch (data.payloadType)
            {
                case PayloadType.Image:
                    pattern = ".png";
                    break;
                case PayloadType.Audio:
                    pattern = ".wav";
                    break;
                case PayloadType.Video:
                    pattern = ".avi";
                    break;
            }

            if (Directory.Exists(url))
            {
                string[] fileEntries = Directory.GetFiles(url);
                foreach (string file in fileEntries)
                {
                    if (!file.Contains(pattern))
                        continue;
                    string newPath = file.Replace("\\", "/");
                    SceneItemElementViewController controller =
                        m_ItemElementFactory.Create(data, newPath, sceneItemElementPrefab);
                    controller?.transform.SetParent(m_List.Content, false);
                }
            }
            else
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(assetsUrl);
                WebResponse response = request.GetResponse();

                Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    Debug.Log(result);
                    MatchCollection matches = regex.Matches(result);
                    foreach (Match match in matches)
                    {
                        if (!match.Success || !match.Value.Contains(pattern))
                            continue;
                        SceneItemElementViewController controller =
                            m_ItemElementFactory.Create(data, assetsUrl + match.Value, sceneItemElementPrefab);
                        controller?.transform.SetParent(m_List.Content, false);
                    }
                }
            }
        }
    }
}