namespace Enigma.Modelling
{
    public interface IPropertyMap
    {
        string PropertyName { get; }
        string Name { get; set; }
        int Index { get; set; }
    }

    public interface IPropertyMap<T> : IPropertyMap
    {
    }
}
