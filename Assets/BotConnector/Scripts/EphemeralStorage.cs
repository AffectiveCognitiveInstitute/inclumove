using RSG;
using System.Collections.Generic;

namespace BotConnector.Unity
{
    /// <summary>
    /// Stores nothing.
    /// </summary>
    public class EphemeralStorage : IBotStorage
    {
        /// <inheritdoc/>
        public IStore Store { get; set; } = new DictionaryStore();

        #region IBotStorage members

        /// <inheritdoc/>
        public IPromise<IList<string>> Bots()
        {
            return Promise<IList<string>>.Resolved(new List<string>());
        }

        /// <inheritdoc/>
        public IPromise<IStore> Load()
        {
            return Promise<IStore>.Resolved(new DictionaryStore());
        }

        /// <inheritdoc/>
        public IPromise Remove()
        {
            return Promise.Resolved();
        }

        /// <inheritdoc/>
        public IPromise Save()
        {
            return Promise.Resolved();
        }

        #endregion
    }
}
