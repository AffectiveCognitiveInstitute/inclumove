using System;
using UnityEngine;

namespace Aci.Unity.Data.JsonModel
{
    [Serializable]
    public class BadgeHistoryData : ISerializationCallbackReceiver
    {
        public DateTime[] badgeDate;
        public BadgeData[] badges;

        [SerializeField]
        private int[] m_UnixTimestamps;

        public void OnAfterDeserialize()
        {
            if(m_UnixTimestamps != null)
            {
                badgeDate = new DateTime[m_UnixTimestamps.Length];
                for (int i = 0; i < m_UnixTimestamps.Length; i++)
                {
                    // Unix timestamp is seconds past epoch
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    dateTime = dateTime.AddSeconds(m_UnixTimestamps[i]);
                    badgeDate[i] = dateTime;
                }
            }
        }

        public void OnBeforeSerialize()
        {
            if (badgeDate != null)
            {
                m_UnixTimestamps = new int[badgeDate.Length];
                for (int i = 0; i < badgeDate.Length; i++)
                    m_UnixTimestamps[i] = (Int32)(badgeDate[i].Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }
    }
}