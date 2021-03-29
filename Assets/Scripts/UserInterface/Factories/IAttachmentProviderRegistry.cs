namespace Aci.Unity.UserInterface.Factories
{
    public interface IAttachmentProviderRegistry
    {
        /// <summary>
        /// Registers an <see cref="IAttachmentProvider"/>
        /// </summary>
        /// <param name="provider">The provider to be registered</param>
        void Register(IAttachmentProvider provider);

        /// <summary>
        /// Unregisters an <see cref="IAttachmentProvider"/>
        /// </summary>
        /// <param name="provider">The provider to be unregistered</param>
        void Unregister(IAttachmentProvider provider);
                
        /// <summary>
        /// Tries to get an <see cref="IAttachmentProvider"/> by content type
        /// </summary>
        /// <param name="contentType">The content type</param>
        /// <param name="provider">The provider</param>
        /// <returns>Returns true if an <see cref="IAttachmentProvider"/> can be found</returns>
        bool TryGetProvider(string contentType, out IAttachmentProvider provider);
    }
}
