using System.Collections.Generic;

namespace Taskington.Base.Extension;

public interface IKeyedRegistry<T>
{
    IEnumerable<T> All { get; }
    void Add(string key, T instance);
    T? Get(string key);
}

public class KeyedRegistry<T> : IKeyedRegistry<T>
{
    private Dictionary<string, T> instances = new();

    public IEnumerable<T> All => instances.Values;

    public void Add(string key, T instance) => instances.Add(key, instance);
    public T? Get(string key) => instances.GetValueOrDefault(key);
}