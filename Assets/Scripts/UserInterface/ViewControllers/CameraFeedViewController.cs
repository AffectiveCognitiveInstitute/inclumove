// <copyright file=CameraFeedViewController.cs/>
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
//   James Gay
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>08/24/2018 13:28</date>

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Aci.Unity.UserInterface.ViewControllers
{
    /// <summary>
    /// Class which displays a camera feed on a RawImage
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class CameraFeedViewController : MonoBehaviour
    {
        private RawImage m_RawImage;
        private WebCamTexture m_WebcamTexture;
        private int prevFramerate = 0;
        public Texture2D placeholder;

        private void Awake()
        {
            m_RawImage = GetComponent<RawImage>();
        }

        private void InitializeCameraTexture()
        {
            m_WebcamTexture = new WebCamTexture();
            foreach (WebCamDevice device in WebCamTexture.devices)
            {
                if (device.name.Contains("Kinect"))
                    continue;
                m_WebcamTexture.deviceName = device.name;
                break;
            }
            m_RawImage.texture = m_WebcamTexture;
        }

        private void CorrectUVs()
        {
            Rect uvRect = m_RawImage.uvRect;
            uvRect.width = (float)m_WebcamTexture.height / (float)m_WebcamTexture.width;
            uvRect.x = (1 - uvRect.width) * 0.5f;
            m_RawImage.uvRect = uvRect;
        }

        public async void StartCameraFeed()
        {
            InitializeCameraTexture();
            prevFramerate = Application.targetFrameRate;
            if (prevFramerate != (int) m_WebcamTexture.requestedFPS)
                Application.targetFrameRate = (int) m_WebcamTexture.requestedFPS;
            m_WebcamTexture.Play();
            await WaitUntilWebcamTexturePlays();
            CorrectUVs();
        }

        private IEnumerator WaitUntilWebcamTexturePlays()
        {
            while (!m_WebcamTexture.isPlaying)
                yield return null;
        }

        public void StopCameraFeed()
        {
            m_WebcamTexture?.Stop();
            if (prevFramerate == 0)
                return;
            if (prevFramerate != Application.targetFrameRate)
                Application.targetFrameRate = prevFramerate;
        }
    }
}