using System;
using System.Collections.Generic;

/// <summary>
/// Serializable wrapper for a List.
/// Unity's JsonUtility requires a wrapping class for top-level list serialization.
/// 
/// Usage:
///   var wrapper = new SerializableList&lt;string&gt;(myList);
///   SaveSystem.Save(wrapper, "myList");
///   var loaded = SaveSystem.Load&lt;SerializableList&lt;string&gt;&gt;("myList");
///   List&lt;string&gt; result = loaded.items;
/// </summary>
[Serializable]
public class SerializableList<T>
{
    public List<T> items;

    public SerializableList()
    {
        items = new List<T>();
    }

    public SerializableList(List<T> list)
    {
        items = list;
    }
}

/// <summary>
/// Serializable wrapper for a Dictionary with string keys.
/// Unity's JsonUtility cannot serialize dictionaries directly,
/// so this class stores keys and values as parallel lists.
/// 
/// Usage:
///   var wrapper = new SerializableDictionary&lt;int&gt;(myDict);
///   SaveSystem.Save(wrapper, "myDict");
///   var loaded = SaveSystem.Load&lt;SerializableDictionary&lt;int&gt;&gt;("myDict");
///   Dictionary&lt;string, int&gt; result = loaded.ToDictionary();
/// </summary>
[Serializable]
public class SerializableDictionary<TValue>
{
    public List<string> keys;
    public List<TValue> values;

    public SerializableDictionary()
    {
        keys = new List<string>();
        values = new List<TValue>();
    }

    public SerializableDictionary(Dictionary<string, TValue> dict)
    {
        keys = new List<string>();
        values = new List<TValue>();
        foreach (var pair in dict)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    /// <summary>
    /// Convert back to a standard Dictionary.
    /// </summary>
    public Dictionary<string, TValue> ToDictionary()
    {
        var dict = new Dictionary<string, TValue>();

        if (keys.Count != values.Count)
        {
            UnityEngine.Debug.LogWarning(
                $"[SerializableDictionary] Key count ({keys.Count}) does not match value count ({values.Count}). Data may be corrupted.");
        }

        int count = Math.Min(keys.Count, values.Count);
        for (int i = 0; i < count; i++)
            dict[keys[i]] = values[i];
        return dict;
    }
}
