using System;
using UnityEngine;
using Zenject;

namespace Aci.Unity.Data.JsonModel
{
    public class ActivityDataFactory : IFactory<string, ActivityData>
    {
        public ActivityData Create(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
                throw new ArgumentNullException(nameof(param));

            ActivityData data = ActivityData.Empty;

            try
            {
                data = JsonUtility.FromJson<ActivityData>(param);
            }
            catch(ArgumentException)
            {
                // Invalid Json
                throw;
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }

            return data;
        }
    }
}
