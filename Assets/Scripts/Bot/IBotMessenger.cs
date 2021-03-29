using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;

namespace Aci.Unity.Bot
{
    public interface IBotMessenger
    {
        Task ResendLastActivityAsync();
        Task SendMessageAsync(string name, string message);
        Task SendEventAsync(string name, object value);
        Task SendActivityAsync(Activity activity);
    }
}
