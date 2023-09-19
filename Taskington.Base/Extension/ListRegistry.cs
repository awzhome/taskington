using System.Collections.Generic;

namespace Taskington.Base.Extension;

public interface IListRegistry<T>
{
    IEnumerable<T> All { get; }
    void Add(T instance);
}

public class ListRegistry<T> : IListRegistry<T>
{
    private List<T> instances = new();

    public IEnumerable<T> All => instances;

    public void Add(T instance) => instances.Add(instance);
}