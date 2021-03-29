using System;
using UnityEngine;

namespace BotConnector.Unity.UI
{
    [Serializable]
    public class AttachmentHandler
    {
        /// <summary>
        /// The prefab for the attachement
        /// </summary>
        public GameObject Template;

        /// <summary>
        /// The type of content, e.g. 'application/vnd.microsoft.card.audio'
        /// </summary>
        public string ContentType;
    }

}

