// <copyright file=SimpleAchievement.cs/>
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
// <date>03/07/2019 09:10</date>

using Aci.Unity.Gamification;
using Aci.Unity.UserInterface;
using Aci.Unity.Util;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Gamification
{
    /// <summary>
    ///     A simple achievement that will always be displayed.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Simple Achievement")]
    public class SimpleAchievement : AciAchievement
    {
        public GameObject achievementPrefab;

        public Color badgeColor;
        public Sprite badgeSprite;
        public string achievementTitle;
        public string AchievementSubtitle;

        /// <inheritdoc />
        public override AchievementBadge Evaluate(DiContainer installer, ITimeProvider timeProvider)
        {
            GameObject newObj = installer.InstantiatePrefab(achievementPrefab);
            AchievementBadge badge = newObj.GetComponent<AchievementBadge>();

            badge.achievementText.text = AchievementSubtitle;
            badge.achievementText.color = Color.white;
            badge.achievementTitle.text = achievementTitle;
            badge.achievementTitle.color = Color.white;
            badge.badgeImage.sprite = badgeSprite;
            badge.badgeImage.color = Color.white;
            badge.badgeRim.color = Color.white;
            badge.badgeBackground.color = badgeColor;

            return badge;
        }
    }
}