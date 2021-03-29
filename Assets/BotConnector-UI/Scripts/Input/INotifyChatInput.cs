using System;

namespace BotConnector.Unity.UI
{
    public interface INotifyChatInput
    {
        event Action<string> ChatInputReceived;
    }
}

