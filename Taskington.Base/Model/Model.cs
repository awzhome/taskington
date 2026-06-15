using System.Collections.Generic;
using System.Linq;

namespace Taskington.Base.Model;

public abstract class Model(IEnumerable<KeyValuePair<string, string>> initialProperties)
{
    private readonly Dictionary<string, string> properties = new(initialProperties);

    protected Model() : this([])
    {
    }

    public string? this[string name]
    {
        get
        {
            if (properties.TryGetValue(name, out string? value))
            {
                return value;
            }

            return null;
        }
        set
        {
            if (value != null)
            {
                properties[name] = value;
            }
            else
            {
                properties.Remove(name);
            }
        }
    }

    public IEnumerable<KeyValuePair<string, string>> Properties => properties;
}