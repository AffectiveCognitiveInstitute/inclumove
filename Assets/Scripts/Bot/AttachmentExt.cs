using Microsoft.Bot.Connector.DirectLine;
using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Aci.Unity.Bot
{
    public static class AttachmentExt
    {
        public static T GetRichCard<T>(this Attachment a)
        {
            try
            {
                JContainer container = a.Content as JContainer;
                return container.Root.ToObject<T>();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return default(T);
        }
    }
}
