// <copyright file=IAciUser.cs/>
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
using Aci.Unity.Data;
using Aci.Unity.Data.JsonModel;
using UnityEngine;

namespace Aci.Unity.Gamification
{
    public interface IUserProfile
    {
        string userName { get; set; }

        Sprite userPicture { get; set; }

        string[] workflows { get; }

        bool isAdmin { get; }

        byte adaptivityLevel { get; set; }

        IReadOnlyList<DateTime> badgeDates { get; }

        IReadOnlyList<BadgeData> badges { get; }

        IReadOnlyList<string> activatedMilestones { get; }

        System.TimeSpan accumulatedTime { get; set; }

        string lastLoginDate { get; set; }

        int numberOfDaysWorked { get; set; }

        int totalBadgeCount { get; set; }

        string selectedReward { get; set; }

        void AddBadgeDataToHistory(BadgeData data);

        void AddActivatedMilestone(string id);

        void RemoveActivatedMilestone(string id);

        bool IsMilestoneActivated(string id);

        UserProfileData ToData();
    }
}