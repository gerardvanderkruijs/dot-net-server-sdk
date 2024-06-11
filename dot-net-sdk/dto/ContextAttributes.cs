using System.Collections;
using System.Diagnostics.CodeAnalysis;
using eppo_sdk.dto.bandit;
using eppo_sdk.exception;

namespace eppo_sdk.dto;

/// A set of attributes (string and numbers only) for a given context `Key`.
public interface IContextAttributes : IDictionary<string, object>
{
    public string Key { get; init; }
};

public class ContextAttributes : IContextAttributes
{

    public string Key { get; init; }

    private readonly Dictionary<string, object> _internalDictionary = new();


    public ContextAttributes(string key)
    {
        this.Key = key;
    }

    /// Adds a value to the subject dictionary enforcing only string and numeric values.
    public void Add(string key, object value)
    {
        // Implement your custom validation logic here
        if (IsNumeric(value) || value is string)
        {
            _internalDictionary.Add(key, value);
        }
        else
        {
            throw new InvalidAttributeTypeException(key, value);
        }
    }

    public DoubleDictionary GetNumeric()
    {
        var nums = this.Where(kvp => IsNumeric(kvp.Value));
        return (DoubleDictionary)nums.ToDictionary(kvp => kvp.Key, kvp => Convert.ToDouble(kvp.Value));
    }
    public StringDictionary GetCategorical()
    {
        var cats = this.Where(kvp => kvp.Value is string);
        return (StringDictionary)cats.ToDictionary(kvp => kvp.Key, kvp => (String)kvp.Value);
    }

    public static bool IsNumeric(object v) => v is double || v is int || v is long || v is float;


    // Standard Dictionary methods are "sealed" so overriding isn't possible. Thus we delegate everything here.

    public object this[string key] { get => _internalDictionary[key]; set => _internalDictionary[key] = value; }

    public ICollection<string> Keys => _internalDictionary.Keys;

    public ICollection<object> Values => _internalDictionary.Values;

    public int Count => _internalDictionary.Count;

    public bool IsReadOnly => false;

    public void Add(KeyValuePair<string, object> item) => Add(item.Key, item.Value);

    public void Clear() => _internalDictionary.Clear();

    public bool Contains(KeyValuePair<string, object> item) => _internalDictionary.ContainsKey(item.Key) && _internalDictionary[item.Key].Equals(item.Value);

    public bool ContainsKey(string key) => _internalDictionary.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _internalDictionary.GetEnumerator();

    public bool Remove(string key) => _internalDictionary.Remove(key);

    public bool Remove(KeyValuePair<string, object> item) => _internalDictionary.Remove(item.Key); // Assuming removal by key

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => _internalDictionary.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => ((IDictionary<string, object>)_internalDictionary).CopyTo(array, arrayIndex);
}
