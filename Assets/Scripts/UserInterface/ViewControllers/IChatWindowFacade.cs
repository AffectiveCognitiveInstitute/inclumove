using UnityEngine;

namespace Aci.Unity.UserInterface
{
    /// <summary>
    ///     Facade which acts as a wrapper for <see cref="IChatWindow"/> methods so 
    ///     we can bind this to Zenject before an instance of IChatWindow exists.
    /// </summary>
    public interface IChatWindowFacade
    {
        /// <summary>
        ///     Register an instance of <see cref="IChatWindow"/>
        /// </summary>
        /// <param name="chatWindow">The chat window</param>
        void Register(IChatWindow chatWindow);

        /// <summary>
        ///     Unregisters an instance of <see cref="IChatWindow"/>
        /// </summary>
        /// <param name="chatWindow"></param>
        void Unregister();

        /// <summary>
        ///     Clears the current chat window
        /// </summary>
        void Clear();

        /// <summary>
        ///     Adds a message to the chat
        /// </summary>
        /// <param name="gameObject">The GameObject representing the chat message</param>
        void AddMessage(GameObject gameObject);
    }
}