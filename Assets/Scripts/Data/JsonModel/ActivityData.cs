using Aci.Unity.UserInterface.Factories;
using Zenject;

namespace Aci.Unity.Data.JsonModel
{
    [System.Serializable]
    public struct ActivityData
    {
        public class Factory : PlaceholderFactory<string, ActivityData> { }

        public static ActivityData Empty = new ActivityData()
        {
            message = "",
            speechFilePath = "",
            mediaFilePath = "",
            contentType = AttachmentContentType.Default
        };

        public string message;
        public string speechFilePath;
        public string mediaFilePath;
        public string contentType;
    }
}
