using System;

namespace Aci.Unity.Data.JsonModel
{
    [Serializable]
    public struct UserProfileData
    {
        public string name;
        public byte[] profilePicture;
        public bool isAdmin;
        public string[] workflows;
        public BadgeHistoryData badgeHistory;
        public byte adaptivityLevel;
        public System.TimeSpan accumulatedTime;
        public string lastLoginDate;
        public int numberOfDaysWorked;
        public int totalBadgeCount;
        //milestones
        public string[] activatedMilestones;
        //settings
        public string selectedReward;
    }
}