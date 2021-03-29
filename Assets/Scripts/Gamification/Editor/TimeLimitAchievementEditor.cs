using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Aci.Unity.Gamification.Editor
{
    [CustomEditor(typeof(TimeLimitAchievement))]
    public class TimeLimitAchievementEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            TimeLimitAchievement achievementData = (TimeLimitAchievement) target;

            EditorGUILayout.BeginVertical();

            bool dirty = false;

            achievementData.achievementPrefab = EditorGUILayout.ObjectField("Badge Prefab", achievementData.achievementPrefab, typeof(GameObject), false) as GameObject;
            int newTiers = EditorGUILayout.IntField("Tiers", achievementData.tiers);
            if (achievementData.tiers != newTiers)
            {
                achievementData.SetTiers(newTiers);
                dirty = true;
            }

            for (int i = 0; i < achievementData.tiers; ++i)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Tier " + i);
                float limit = EditorGUILayout.DelayedFloatField("Time Limit (sec)", achievementData.tierDurationLimits[i]);
                if (limit != achievementData.tierDurationLimits[i])
                {
                    achievementData.tierDurationLimits[i] = limit;
                    dirty = true;
                }
                string title = EditorGUILayout.DelayedTextField("Tier Title", achievementData.tierTitles[i]);
                if (title != achievementData.tierTitles[i])
                {
                    achievementData.tierTitles[i] = title;
                    dirty = true;
                }
                string subtitle = EditorGUILayout.DelayedTextField("Tier Subtitle", achievementData.tierSubTitles[i]);
                if (subtitle != achievementData.tierSubTitles[i])
                {
                    achievementData.tierSubTitles[i] = subtitle;
                    dirty = true;
                }
                Sprite icon = EditorGUILayout.ObjectField("Tier Icon", achievementData.tierIcons[i], typeof(Sprite), false) as Sprite;
                if (icon != achievementData.tierIcons[i])
                {
                    achievementData.tierIcons[i] = icon;
                    dirty = true;
                }
                Color color= EditorGUILayout.ColorField("Tier Color", achievementData.tierColors[i]);
                if (color != achievementData.tierColors[i])
                {
                    achievementData.tierColors[i] = color;
                    dirty = true;
                }
            }

            EditorGUILayout.EndVertical();

            if (!dirty)
                return;

            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }
    }

}
