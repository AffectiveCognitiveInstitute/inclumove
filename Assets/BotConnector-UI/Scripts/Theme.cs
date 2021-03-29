using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BotConnector.Unity.UI
{
    [CreateAssetMenu(menuName = "BotConnector/Theme")]
    public class Theme : ScriptableObject
    {
        // General
        public GameObject BotActivity;
        public GameObject UserActivity;

        // Handler
        [SerializeField]
        private AttachmentHandler[] attachments;

        // Controls
        public GameObject Button;
        public GameObject SuggestedAction;
        public GameObject Carousel;
        public GameObject List;
        public GameObject Separator;

        private Dictionary<string, AttachmentHandler> internalAttachments;

        public IDictionary<string, AttachmentHandler> Attachments
        {
            get
            {
                if (internalAttachments == null)
                    internalAttachments = attachments.ToDictionary(x => x.ContentType);
                return internalAttachments;
            }
        }

    }
}

