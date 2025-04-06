using System.Collections.Generic;

namespace SimToolAI.Utilities
{
    /// <summary>
    /// Generic data container for returning information from queries
    /// </summary>
    public class Data
    {
        private readonly Dictionary<string, object> _properties = new();

        public void Set<T>(string key, T value)
        {
            _properties[key] = value;
        }

        public T Get<T>(string key)
        {
            if (_properties.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }

        public bool Has(string key)
        {
            return _properties.ContainsKey(key);
        }
    }
}