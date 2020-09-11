using System.Collections.Generic;
using System.Text;

namespace PPBackup.Base.SystemOperations
{
    public class Placeholders
    {
        private readonly Dictionary<string, string> placeholderMappings = new Dictionary<string, string>();

        public string? this[string placeholder]
        {
            get
            {
                if (placeholderMappings.TryGetValue(placeholder, out string val))
                {
                    return val;
                }

                return null;
            }

            set
            {
                if (value == null)
                {
                    placeholderMappings.Remove(placeholder);
                }
                else
                {
                    placeholderMappings[placeholder] = value;
                }
            }
        }

        public string? ResolvePlaceholders(string? input)
        {
            if (input == null)
            {
                return null;
            }

            var output = new StringBuilder();

            int pos = 0;
            while (pos < input.Length)
            {
                int placeholderStart = input.IndexOf("${", pos);
                if (placeholderStart == -1)
                {
                    output.Append(input.Substring(pos));
                    break;
                }
                else
                {
                    output.Append(input[pos..placeholderStart]);

                    int placeholderEnd = input.IndexOf('}', placeholderStart);
                    if (placeholderEnd == -1)
                    {
                        output.Append(input.Substring(placeholderStart));
                        break;
                    }
                    else
                    {
                        string placeholder = input.Substring(placeholderStart + 2, placeholderEnd - placeholderStart - 2);
                        string? placeholderValue = this[placeholder];
                        if (placeholderValue != null)
                        {
                            output.Append(placeholderValue);
                        }
                        else
                        {
                            output.Append("${" + placeholder + "}");
                        }

                        pos = placeholderEnd + 1;
                    }
                }

            }

            return output.ToString();
        }

        public IEnumerable<(string Placeholder, string? Resolved)> ExtractPlaceholders(string input)
        {
            var foundPlaceholders = new HashSet<string>();

            int pos = 0;
            while (pos < input.Length)
            {
                int placeholderStart = input.IndexOf("${", pos);
                if (placeholderStart == -1)
                {
                    break;
                }
                else
                {
                    int placeholderEnd = input.IndexOf('}', placeholderStart);
                    if (placeholderEnd == -1)
                    {
                        break;
                    }
                    else
                    {
                        string placeholder = input.Substring(placeholderStart + 2, placeholderEnd - placeholderStart - 2);
                        if (!foundPlaceholders.Contains(placeholder))
                        {
                            yield return (placeholder, this[placeholder]);
                            foundPlaceholders.Add(placeholder);
                        }

                        pos = placeholderEnd + 1;
                    }
                }
            }
        }
    }
}
