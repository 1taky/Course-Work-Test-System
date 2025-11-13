using DAL.Interfaces;
public class InMemoryProviderStub<T> : IDataProvider<T>
{
    private List<T> _data = new();

    public void Save(List<T> data)
    {
        _data = data;
    }

    public List<T> Load()
    {
        return _data;
    }
}