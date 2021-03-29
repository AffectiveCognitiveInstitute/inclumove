using UnityEngine;

namespace Aci.Unity.UserInterface
{
    public interface IChatWindow
    {
        /// <summary>
        ///     Clears the chat
        /// </summary>
        void Clear();

        /// <summary>
        ///     Adds a message to the chat window
        /// </summary>
        /// <param name="gameObject">The GameObject representing the chat message</param>
        void AddMessage(GameObject gameObject);
    }
}