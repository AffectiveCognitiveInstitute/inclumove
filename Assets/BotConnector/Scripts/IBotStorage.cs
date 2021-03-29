using RSG;
using System.Collections.Generic;

namespace BotConnector.Unity
{
    /// <summary>
    /// Handles storage of bots.
    /// </summary>
    public interface IBotStorage
    {
        IStore Store { get; }

        /// <summary>
        /// Gets a list of all stored bots.
        /// </summary>
        /// <returns></returns>
        IPromise<IList<string>> Bots();

        /// <summary>
        /// Loads data for a bot.
        /// </summary>
        /// <returns>A promise that will resolve to the store for the specified bot.</returns>
        IPromise<IStore> Load();

        /// <summary>
        /// Saves data for a bot.
        /// </summary>
        /// <param name="values">The data to store.</param>
        /// <returns>A promise that represents the store-operation.</returns>
        IPromise Save();

        /// <summary>
        /// Removes all data for a bot.
        /// </summary>
        /// <param name="bot">The bot handle.</param>
        /// <returns>A promise that represents the remove-operation.</returns>
        IPromise Remove();
    }

    /// <summary>
    /// Stores data.
    /// </summary>
    public interface IStore
    {
        string this[string key] { get; set; }

        bool ContainsKey(string key);

        void AddOrReplace(string key, string value);
    }

    /// <summary>
    /// Stores data using a <see cref="Dictionary{string, string}"/>.
    /// </summary>
    public class DictionaryStore : Dictionary<string, string>, IStore
    {
        public void AddOrReplace(string key, string value)
        {
            if (ContainsKey(key))
                this[key] = value;
            else
                Add(key, value);
        }
    }

}

