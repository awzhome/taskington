using System.Collections.Generic;
using System.Linq;

namespace Taskington.Base.Model
{
    public abstract class Model
    {
        private readonly Dictionary<string, string> properties;

        public Model() : this(Enumerable.Empty<KeyValuePair<string, string>>())
        {
        }

        public Model(IEnumerable<KeyValuePair<string, string>> initialProperties)
        {
            properties = new(initialProperties);
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
}
