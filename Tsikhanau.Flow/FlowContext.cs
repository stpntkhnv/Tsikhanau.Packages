namespace Tsikhanau.Flow;

public class FlowContext
{
    private readonly Dictionary<String, Object> _data;
    private readonly Dictionary<String, Object> _metadata;

    public FlowContext()
    {
        _data = new Dictionary<String, Object>();
        _metadata = new Dictionary<String, Object>();
    }

    public FlowContext(FlowContext parent) : this()
    {
        foreach (var kvp in parent._data)
        {
            _data[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in parent._metadata)
        {
            _metadata[kvp.Key] = kvp.Value;
        }
    }

    public T Get<T>(String key)
    {
        if (_data.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }

        return default(T)!;
    }

    public T? GetOrDefault<T>(String key, T? defaultValue = default)
    {
        if (_data.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }

        return defaultValue;
    }

    public void Set<T>(String key, T value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        _data[key] = value;
    }

    public Boolean TryGet<T>(String key, out T value)
    {
        if (_data.TryGetValue(key, out var obj) && obj is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default(T)!;
        return false;
    }

    public Boolean Contains(String key) => _data.ContainsKey(key);

    public void Remove(String key) => _data.Remove(key);

    public void Clear() => _data.Clear();

    public IReadOnlyDictionary<String, Object> Data => _data.AsReadOnly();

    public T GetMetadata<T>(String key)
    {
        if (_metadata.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }

        return default(T)!;
    }

    public void SetMetadata<T>(String key, T value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        _metadata[key] = value;
    }

    public Boolean TryGetMetadata<T>(String key, out T value)
    {
        if (_metadata.TryGetValue(key, out var obj) && obj is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default(T)!;
        return false;
    }

    public IReadOnlyDictionary<String, Object> Metadata => _metadata.AsReadOnly();
}