// <copyright file=AciUser.cs/>
// <copyright>
//   Copyright (c) 2018, Affective & Cognitive Institute
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
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
using System.Collections.Generic;
using UnityEngine;
using Aci.Unity.Data;
using Aci.Unity.Data.JsonModel;

namespace Aci.Unity.Gamification
{
    public class UserProfile : IUserProfile
    {
        private Sprite m_Sprite;

        public string userName { get; set; }
        public Sprite userPicture
        {
            get => m_Sprite;
            set => m_Sprite = value;
        }

        public System.TimeSpan accumulatedTime { get; set; }

        public string lastLoginDate { get; set; }

        public int numberOfDaysWorked { get; set; }

        public string[] workflows { get; private set; }

        private List<DateTime> m_BadgeDates = new List<DateTime>();
        public IReadOnlyList<DateTime> badgeDates => m_BadgeDates.AsReadOnly();

        private List<BadgeData> m_BadgeData = new List<BadgeData>();
        public IReadOnlyList<BadgeData> badges => m_BadgeData.AsReadOnly();

        private List<string> m_ActivatedMilestones = new List<string>();

        public IReadOnlyList<string> activatedMilestones => m_ActivatedMilestones;

        public bool isAdmin { get; private set; } = false;
        public byte adaptivityLevel { get; set; }
        public int totalBadgeCount { get; set; }
        public string selectedReward { get; set; }

        public void AddBadgeDataToHistory(BadgeData data)
        {
            if (m_BadgeData.Count > 5)
            {
                m_BadgeDates.RemoveAt(0);
                m_BadgeData.RemoveAt(0);
            }
            m_BadgeDates.Add(DateTime.UtcNow);
            m_BadgeData.Add(data);
        }

        public void AddActivatedMilestone(string id)
        {
            if (!m_ActivatedMilestones.Contains(id))
                m_ActivatedMilestones.Add(id);
        }

        public void RemoveActivatedMilestone(string id) => m_ActivatedMilestones.Remove(id);

        public bool IsMilestoneActivated(string id) => m_ActivatedMilestones.Contains(id);

        public UserProfile(UserProfileData data)
        {
            userName = data.name;
            if(data.profilePicture != null)
                CreateSpriteFromBytes(data.profilePicture);
            workflows = data.workflows;
            isAdmin = data.isAdmin;
            adaptivityLevel = data.adaptivityLevel;
            if(data.badgeHistory.badgeDate != null)
            {
                m_BadgeDates.AddRange(data.badgeHistory.badgeDate);
                m_BadgeData.AddRange(data.badgeHistory.badges);
            }
            accumulatedTime = data.accumulatedTime;
            lastLoginDate = data.lastLoginDate;
            numberOfDaysWorked = data.numberOfDaysWorked;
            totalBadgeCount = data.totalBadgeCount;
            
            m_ActivatedMilestones = data.activatedMilestones != null? new List<string>(data.activatedMilestones) : new List<string>();                
            if (data.selectedReward.Length == 0)
                data.selectedReward = "Balloons";
            selectedReward = data.selectedReward;
        }

        public UserProfileData ToData()
        {
            Texture2D tex = userPicture?.texture;
            return new UserProfileData()
            {
                name = userName,
                profilePicture = tex.EncodeToPNG(),
                isAdmin = this.isAdmin,
                workflows = this.workflows,
                badgeHistory = new BadgeHistoryData() { badgeDate = m_BadgeDates.ToArray(), badges = m_BadgeData.ToArray() },
                accumulatedTime = this.accumulatedTime,
                lastLoginDate = this.lastLoginDate,
                numberOfDaysWorked = this.numberOfDaysWorked,
                totalBadgeCount = this.totalBadgeCount,
                selectedReward = this.selectedReward,
                activatedMilestones = m_ActivatedMilestones.ToArray()
            };
        }
        
        private void CreateSpriteFromBytes(byte[] data)
        {
            Texture2D tex = new Texture2D(1,1,TextureFormat.ARGB32,false);
            tex.LoadImage(data, false);
            tex.Apply();
            m_Sprite = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), Vector2.one * 0.5f);
        }
    }
}