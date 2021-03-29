namespace Aci.Unity.Data
{
    public interface IIdProviderService<T>
    {
        T Register(IIdentifiable<T> identifiable);

        void Unregister(IIdentifiable<T> identifiable);

        bool HasId(T id);
    }
}
