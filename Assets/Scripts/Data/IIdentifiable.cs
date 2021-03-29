namespace Aci.Unity.Data
{
    public interface IIdentifiable<T>
    {
        T identifier { get; set; }
    }
}
