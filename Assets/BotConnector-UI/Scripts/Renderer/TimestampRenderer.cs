using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BotConnector.Unity.UI
{
    /// <summary>
    /// Renders a self-updating timestamp.
    /// </summary>
    public class TimestampRenderer : RendererBase<DateTime>
    {
        public Text Output;

        private void Awake()
        {
            Output = GetComponent<Text>();
        }

        private IEnumerator RenderTimestamp(DateTime timestamp)
        {
            var result = GetTimestampInfo(timestamp);
            Output.text = result.Item1;

            if (result.Item2.HasValue)
            {
                yield return new WaitForSeconds(result.Item2.Value);
                StartCoroutine(RenderTimestamp(timestamp));
            }
        }

        public override void Render(DateTime content, IRenderContext context)
        {
            StartCoroutine(RenderTimestamp(content));
        }

        /// <summary>
        /// Creates a tuple that contains a friendly format of the timestamp and 
        /// the amount of seconds to wait for refreshing.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns></returns>
        public static Tuple<string, int?> GetTimestampInfo(DateTime timestamp)
        {
            var ts = DateTime.UtcNow - timestamp;

            if (ts.TotalMinutes < 1)
                return Tuple("Now", 60);

            if (Math.Floor(ts.TotalMinutes) == 1)
                return Tuple("1 minute", 60);

            if (ts.TotalHours < 1)
                return Tuple($"{ts.Minutes} minutes", 60);

            if (Math.Floor(ts.TotalHours) == 1)
                return Tuple("1 hour", 60 * 60);

            if (ts.TotalHours < 5)
                return Tuple($"{ts.Hours} hours", 60 * 60 * (5 - ts.Hours));

            if (ts.TotalHours <= 24)
                return Tuple("today", 60 * 60 * (24 - ts.Hours));

            if (ts.TotalHours <= 48)
                return Tuple("yesterday", 60 * 60 * (48 - ts.Hours));

            return Tuple(timestamp.ToShortDateString(), null);
        }

        private static Tuple<string, int?> Tuple(string s, int? i)
        {
            return new Tuple<string, int?>(s, i);
        }        
    }

}

