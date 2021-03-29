// <copyright file=TimeLimitAchievement.cs/>
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
// <date>08/20/2018 11:33</date>

using System;
using System.Collections.Generic;
using Aci.Unity.UserInterface;
using Aci.Unity.UI.Localization;
using Aci.Unity.Util;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Gamification
{
    [ExecuteInEditMode]
    public class TimeLimitAchievement : AciAchievement
    {
        public GameObject achievementPrefab;

        public int tiers => tierDurationLimits.Count;
        public List<float> tierDurationLimits = new List<float>();
        public List<Color> tierColors = new List<Color>();
        public List<Sprite> tierIcons = new List<Sprite>();
        public List<string> tierSubTitles = new List<string>();
        public List<string> tierTitles    = new List<string>();

        public void SetTiers(int numTiers)
        {

            int diff = tierDurationLimits.Count - tiers;
            while (diff > 0)
            {
                tierDurationLimits.Add(0f);
                tierIcons.Add(null);
                tierColors.Add(Color.white);
                tierTitles.Add("");
                tierSubTitles.Add("");
                --diff;
            }
            while (diff < 0)
            {
                int ndx = tierDurationLimits.Count - 1;
                tierDurationLimits.RemoveAt(ndx);
                tierIcons.RemoveAt(ndx);
                tierColors.RemoveAt(ndx);
                tierTitles.RemoveAt(ndx);
                tierSubTitles.RemoveAt(ndx);
                ++diff;
            }
        }

        /// <inheritdoc />
        public override AchievementBadge Evaluate(DiContainer installer, ITimeProvider timeProvider)
        {
            GameObject newObj = installer.InstantiatePrefab(achievementPrefab);
            AchievementBadge badge = newObj.GetComponent<AchievementBadge>();

            int chosen = tiers - 1;
            for (int i = 0; i < tiers; ++i)
            {
                if (timeProvider.elapsedTotal.TotalSeconds > tierDurationLimits[i])
                    continue;
                chosen = i;
            }

            badge.achievementText.text = tierSubTitles[chosen];
            badge.achievementText.color = Color.white;
            badge.achievementTitle.text = tierTitles[chosen];
            badge.achievementTitle.color = Color.white;
            badge.badgeImage.sprite = tierIcons[chosen];
            badge.badgeImage.color = Color.white;
            badge.badgeRim.color = Color.white;
            badge.badgeBackground.color = tierColors[chosen];

            return badge;
        }
    }
}