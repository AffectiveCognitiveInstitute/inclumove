// <copyright file=UserAvatarRecorder.cs/>
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
// <date>07/23/2018 15:17</date>

using System;
using System.Threading.Tasks;
using Aci.Unity.Audio;
using Aci.Unity.Events;
using Aci.Unity.Gamification;
using Aci.Unity.Network;
using Aci.Unity.Networking;
using Aci.Unity.Sensor;
using Aci.Unity.UI.Navigation;
using Aci.Unity.UI.Tweening;
using Aci.Unity.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Aci.Unity.UserInterface
{
    public class RegistrationViewController : MonoBehaviour
    {
        private IUserManager m_UserManager;
        private IAudioService m_AudioService;
        private INavigationService m_NavigationService;
        private IMilestoneFacade m_MilestoneFacade;
        private UsbDetectorService m_UsbService;


        [SerializeField]
        private TextMeshProUGUI userName;

        [SerializeField]
        private RawImage targetImage;

        [SerializeField]
        private Sprite placeholder;

        [SerializeField]
        private AudioClip tickClip;

        [SerializeField]
        private AudioClip snapClip;

        [Inject]
        public void Construct(IUserManager userManager,
                              IAudioService audioService,
                              INavigationService navigationService,
                              IMilestoneFacade milestoneFacade,
                              UsbDetectorService usbService)
        {
            m_UserManager = userManager;
            m_AudioService = audioService;
            m_NavigationService = navigationService;
            m_MilestoneFacade = milestoneFacade;
            m_UsbService = usbService;
        }

        public async void RegisterNewUser()
        {
            WebCamTexture tex = (targetImage.texture as WebCamTexture);
            Texture2D snap;
            Sprite targetSprite;
            if (!tex.isPlaying)
            {
                targetSprite = placeholder;
                snap = placeholder.texture;
            }
            else
            {
                await Countdown();
                m_AudioService.PlayAudioClip(snapClip, AudioChannels.SFX);
                Rect snapRect = new Rect(tex.width * targetImage.uvRect.x
                                       , tex.height * targetImage.uvRect.y
                                       , tex.width * targetImage.uvRect.width
                                       , tex.height * targetImage.uvRect.height);

                snap = new Texture2D((int)snapRect.width, (int)snapRect.height);
                snap.SetPixels(tex.GetPixels((int)snapRect.x, (int)snapRect.y, (int)snapRect.width,
                                             (int)snapRect.height));
                snap.Apply();
                snap.Resize(100, 100, TextureFormat.ARGB32, false);
                snap.Apply();
                targetSprite = Sprite.Create(snap, new Rect(0, 0, snap.width, snap.height), new Vector2(0.5f, 0.5f));
            }
            m_UserManager.CreateNewUser(userName.text, targetSprite);
            await m_UserManager.SaveUser();
            await m_NavigationService.PushAsync("Welcome01", AnimationOptions.Synchronous, false);
            m_MilestoneFacade.StartMilestoneQuests();
        }

        private async Task RecordUserAvatar(Sprite targetSprite)
        {
            
        }

        private async Task Countdown()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            for (int i = 3; i > 0; --i)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.8f));
                m_AudioService.PlayAudioClip(tickClip, AudioChannels.SFX);
                await Task.Delay(TimeSpan.FromSeconds(0.2f));
            }
            await Task.Delay(TimeSpan.FromSeconds(0.8f));
        }
    }
}