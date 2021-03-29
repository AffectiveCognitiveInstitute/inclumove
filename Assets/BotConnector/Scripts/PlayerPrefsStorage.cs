using RSG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BotConnector.Unity
{
    /// <summary>
    /// Stores information with the <see cref="PlayerPrefs"/> API.
    /// </summary>
    public class PlayerPrefsStorage : MonoBehaviour, IBotStorage
    {
        #region Consts

        private const string BotKey = "BotFramework_Bots";
        private static readonly char[] BotSeparator = { ';' };
        private static readonly char[] BotDataSeparator = { ';', '=' };

        #endregion

        #region Private fields

        private List<string> bots = new List<string>();
        private DictionaryStore store = new DictionaryStore();

        #endregion

        #region Properties

        public string Bot { get; set; }

        public IStore Store { get { return store; } }

        #endregion


        private void Start()
        {
            Bot = Bot ?? GetComponent<IBot>().Handle;
        }

        #region IBotStorage members

        /// <inheritdoc/>
        public IPromise<IList<string>> Bots()
        {
            if (PlayerPrefs.HasKey(BotKey))
            {
                bots = PlayerPrefs.GetString(BotKey)
                    .Split(BotSeparator, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }
            return Promise<IList<string>>.Resolved(bots);
        }

        /// <inheritdoc/>
        public IPromise<IStore> Load()
        {
            if (Bot == null)
                Start();

            if (!PlayerPrefs.HasKey(Bot))
                return Promise<IStore>.Rejected(new ArgumentException($"No bot with handle '{Bot}' stored in PlayerPrefs"));

            return Parse();
        }

        /// <inheritdoc/>
        public IPromise Save()
        {
            SaveBotsList();
            SaveBot();

            return Promise.Resolved();
        }

        /// <inheritdoc/>
        public IPromise Remove()
        {
            if (bots.Contains(Bot))
                bots.Remove(Bot);

            if (PlayerPrefs.HasKey(Bot))
                PlayerPrefs.DeleteKey(Bot);

            return Promise.Resolved();
        }

        #endregion

        #region Private Methods

        private void SaveBot()
        {
            var builder = new StringBuilder();

            store.Where(pair => !string.IsNullOrEmpty(pair.Value))
                .ToList()
                .ForEach(pair => builder.Append(pair.Key).Append("=").Append(pair.Value).Append(";"));
            
            PlayerPrefs.SetString(Bot, builder.ToString());
        }

        private void SaveBotsList()
        {
            var builder = new StringBuilder();
            bots.ForEach(bot => builder.Append(bot).Append(";"));
            PlayerPrefs.SetString(BotKey, builder.ToString());
        }

        private IPromise<IStore> Parse()
        {
            var newStore = new DictionaryStore();
            var values = PlayerPrefs.GetString(Bot).Split(BotDataSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (!IsParsable(values))
                return Promise<IStore>.Rejected(new Exception("Could not parse content for bot store."));

            for (int i = 0; i < values.Length; i++)
            {
                newStore.Add(values[i++], values[i]);
            }

            store = newStore;
            return Promise<IStore>.Resolved(newStore);
        }

        private bool IsParsable(string[] values)
        {
            return values.Length % 2 == 0;
        }

        #endregion

    }
}