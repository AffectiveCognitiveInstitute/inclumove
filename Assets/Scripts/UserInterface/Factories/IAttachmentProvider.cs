using Microsoft.Bot.Connector.DirectLine;
using UnityEngine;

namespace Aci.Unity.UserInterface.Factories
{
    public interface IAttachmentProvider
    {
        /// <summary>
        /// The Content Type this provider is responsible for
        /// </summary>
        string type { get; }

        /// <summary>
        /// Creates the GameObject View of the attachment
        /// </summary>
        /// <param name="activity">The activity</param>
        /// <returns>Returns the Attachment View as a GameObject</returns>
        GameObject GetGameObject(Activity activity);
    }
}
