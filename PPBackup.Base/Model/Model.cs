using System.Collections.Generic;

namespace PPBackup.Base.Model
{
    public abstract class Model
    {
        private readonly Dictionary<string, string> properties = new Dictionary<string, string>();

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
