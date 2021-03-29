using BotConnector.Unity;
using UnityEngine;

public class BotDebugger : MonoBehaviour
{

    public IBot Bot;

    public string Type, Text, Value, Locale;

    private void Start()
    {
        Bot = Bot ?? GetComponent<IBot>();
    }

    public void SendMessageToBot()
    {
        Bot.SendActivityAsync(new Microsoft.Bot.Connector.DirectLine.Activity { Type = Type, Text = Text, Value = Value, Locale = Locale });
        Text = Value = string.Empty;
    }
}
