// <copyright file=UserManager.cs/>
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

using System.Collections;
using System.IO;
using Aci.Unity.Data.JsonModel;
using UnityEngine;
using Aci.Unity.Events;
using Aci.Unity.Util;
using System.Threading.Tasks;

namespace Aci.Unity.Gamification
{
    /// <summary>
    /// JSON based UserManager.
    /// </summary>
    public class UserManager : IUserManager, IAciEventHandler<USBDevice>
    {
        private IConfigProvider m_ConfigProvider;
        private IAciEventManager m_Broker;
        private IUserProfile m_Profile;
        private UsbDetectorService m_UsbService;

        [ConfigValue("standardWorkflow")]
        public string standardWorkflow { get; set; }

        /// <inheritdoc/>
        public IUserProfile CurrentUser
        {
            get => m_Profile;
            set
            {
                if (m_Profile != null)
                {
                    m_Broker.Invoke(new UserArgs()
                    {
                        eventType = UserArgs.UserEventType.Logout
                    });
                }
                m_Profile = value;
                if (m_Profile == null)
                    return;
                m_Broker.Invoke(new UserArgs()
                {
                    eventType = UserArgs.UserEventType.Login
                });
            }
        }

        /// <summary>
        /// Creates a UserManager instance.
        /// </summary>
        /// <param name="manager"><see cref="IAciEventManager"/> that should send UserManager events. </param>
        /// <param name="usbService">Target <see cref="UsbDetectorService"/> to notify of user data device changes.</param>
        [Zenject.Inject]
        public UserManager(IAciEventManager manager, IConfigProvider configProvider, UsbDetectorService usbService)
        {
            m_Broker = manager;
            m_ConfigProvider = configProvider;
            m_UsbService = usbService;
            RegisterForEvents();
        }

        ~UserManager()
        {
            UnregisterFromEvents();
        }
        
        private void LoadUserFromFile(string filePath)
        {
            string stringData = File.ReadAllText(filePath);
            UserProfileData data = JsonUtility.FromJson<UserProfileData>(stringData);
            CurrentUser = new UserProfile(data);
        }

        /// <inheritdoc/>
        public Task<bool> LoadUser()
        {
            string filePath = m_UsbService.currentDrive + "user.prf";
            if (!File.Exists(filePath))
                return Task.FromResult(false);
            LoadUserFromFile(filePath);
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public Task<bool> SaveUser()
        {
            // send user save event to let other providers save user data
            m_Broker.Invoke(new UserArgs() { eventType = UserArgs.UserEventType.Save });
            string filePath = m_UsbService.currentDrive + "user.prf";
            if (CurrentUser == null)
                return Task.FromResult(false);
            string jsonData = JsonUtility.ToJson(CurrentUser.ToData(), true);
            File.WriteAllText(filePath, jsonData);
            return Task.FromResult(true);
        }

        public void CreateNewUser(string name, Sprite profilePicture)
        {
            Texture2D tex = profilePicture?.texture;
            UserProfileData data = new UserProfileData()
            {
                name = name,
                profilePicture = tex.EncodeToPNG(),
                isAdmin = false,
                workflows = new string[] { standardWorkflow },
                badgeHistory = new BadgeHistoryData(),
                adaptivityLevel = 2,
                accumulatedTime = new System.TimeSpan(),
                lastLoginDate = "",
                numberOfDaysWorked = 0,
                totalBadgeCount = 0,
                selectedReward = "",
                activatedMilestones = new string[0]
            };
            CurrentUser = new UserProfile(data);
        }

        /// <inheritdoc/>
        public void RegisterForEvents()
        {
            m_ConfigProvider?.RegisterClient(this);
            m_ConfigProvider?.ClientDirty(this);
            m_Broker.AddHandler(this);
        }

        /// <inheritdoc/>
        public void UnregisterFromEvents()
        {
            m_ConfigProvider.UnregisterClient(this);
            m_Broker.RemoveHandler(this);
        }

        /// <inheritdoc/>
        public async void OnEvent(USBDevice arg)
        {
            if(arg.drive == null)
            {
                CurrentUser = null;
                return;
            }
            bool success = await LoadUser();
            if(!success)
            {
                // react to failed loading
            }
        }
    }
}