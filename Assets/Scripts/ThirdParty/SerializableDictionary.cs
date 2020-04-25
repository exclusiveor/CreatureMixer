using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//
// Unity doesn't know how to serialize a Dictionary
// So this is a simple extension of a dictionary that saves as two lists.
// By Pablo Bollansée

//
// Usage is a little strange though, for some reason you can't use it directly in unity.
// You have to make a non-generic instance of it, and then use it. This is luckily quite easy:
// 
// [System.Serializable]
// class MyDictionary : SerializableDictionary<KeyType, ValueType> {}
//
// Then make an instance of this like this:
//
// [SerializeField]
// private MyDictionary _dictionary = new MyDictionary();
//
// Now you can use it in exactly the same way as a notmal Dictionary. Everything just works.

//[System.Serializable]
//public class SerializableDictionary<TKey,TValue> : 
//	Dictionary<TKey, TValue>, ISerializationCallbackReceiver {

//	// We save the keys and values in two lists because Unity does understand those.
//	[SerializeField, HideInInspector]
//	private List<TKey> _keys;
//	[SerializeField, HideInInspector]
//	private List<TValue> _values;

//	// Before the serialization we fill these lists
//	public void OnBeforeSerialize() {
//		_keys = new List<TKey>(this.Count);
//		_values = new List<TValue>(this.Count);
//		foreach(var kvp in this) {
//			_keys.Add(kvp.Key);
//			_values.Add(kvp.Value);
//		}
//	}

//	// After the serialization we create the dictionary from the two lists
//	public void OnAfterDeserialize() {
//		this.Clear();
//		for (int i=0; i!= Mathf.Min(_keys.Count,_values.Count); i++) {
//			this.Add(_keys[i],_values[i]);
//		}
//	}
//}

[Serializable]
public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    [SerializeField]
    protected List<TKey> keys;
    [SerializeField]
    protected List<TValue> values;
    public List<TKey> Keys { get { return keys; } }
    public List<TValue> Values { get { return values; } }
    public int Count { get { return keys.Count; } }
    public SerializableDictionary() : this(new List<TKey>(), new List<TValue>())
    {
    }
    public SerializableDictionary(List<TKey> keys, List<TValue> values)
    {
        this.keys = keys;
        this.values = values;
    }
    public TValue this[TKey key]
    {
        get
        {
            int index;
            if (!TryGetIndex(key, out index))
            {
                throw new KeyNotFoundException(key.ToString());
            }
            return values[index];
        }
        set
        {
            int index;
            if (!TryGetIndex(key, out index))
            {
                Add(key, value);
            }
            else values[index] = value;
        }
    }
    public void SetKeyAt(int i, TKey value)
    {
        AssertIndexInBounds(i);
        if (value != null && !value.Equals(keys[i]))
            AssertUniqueKey(value);
        keys[i] = value;
    }
    public TKey GetKeyAt(int i)
    {
        AssertIndexInBounds(i);
        return keys[i];
    }
    public void SetValueAt(int i, TValue value)
    {
        AssertIndexInBounds(i);
        values[i] = value;
    }
    public TValue GetValueAt(int i)
    {
        AssertIndexInBounds(i);
        return values[i];
    }
    public KeyValuePair<TKey, TValue> GetPairAt(int i)
    {
        AssertIndexInBounds(i);
        return new KeyValuePair<TKey, TValue>(keys[i], values[i]);
    }
    private void AssertIndexInBounds(int i)
    {
        if (!keys.InBounds(i))
            throw new IndexOutOfRangeException("i");
    }
    public void Clear()
    {
        keys.Clear();
        values.Clear();
    }
    public void Insert(int i, TKey key, TValue value)
    {
        AssertUniqueKey(key);
        if (key == null)
        {
            throw new ArgumentException(string.Format("Dictionary key cannot be null", "!!!"));
        }
        //Assert.ArgumentNotNull(key, "Dictionary key cannot be null");
        keys.Insert(i, key);
        values.Insert(i, value);
    }
    private void AssertUniqueKey(TKey key)
    {
        if (ContainsKey(key))
            throw new ArgumentException(string.Format("There's already a key `{0}` defined in the dictionary", key.ToString()));
    }
    public void Insert(TKey key, TValue value)
    {
        Insert(0, key, value);
    }
    public void Add(TKey key, TValue value)
    {
        Insert(Count, key, value);
    }
    public bool Remove(TKey key)
    {
        int index;
        if (TryGetIndex(key, out index))
        {
            keys.RemoveAt(index);
            values.RemoveAt(index);
            return true;
        }
        return false;
    }
    public void RemoveAt(int i)
    {
        AssertIndexInBounds(i);
        keys.RemoveAt(i);
        values.RemoveAt(i);
    }
    public void RemoveLast()
    {
        RemoveAt(Count - 1);
    }
    public void RemoveFirst()
    {
        RemoveAt(0);
    }
    public bool TryGetValue(TKey key, out TValue result)
    {
        int index;
        if (!TryGetIndex(key, out index))
        {
            result = default(TValue);
            return false;
        }
        result = values[index];
        return true;
    }
    public bool ContainsValue(TValue value)
    {
        return values.Contains(value);
    }
    public bool ContainsKey(TKey key)
    {
        return keys.Contains(key);
    }
    private bool TryGetIndex(TKey key, out int index)
    {
        return (index = keys.IndexOf(key)) != -1;
    }
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return new KeyValuePair<TKey, TValue>(keys[i], values[i]);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
        get { return keys; }
    }
    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
        return Remove(key);
    }
    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
        get { return values; }
    }
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        keys.Add(item.Key);
        values.Add(item.Value);
    }
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ContainsKey(item.Key);
    }
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        for (int i = arrayIndex; i < array.Length; i++)
        {
            array[i] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
        }
    }
    public bool IsReadOnly
    {
        get { return false; }
    }
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }
}

public static class KVPDictionaryExtensions
{
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this SerializableDictionary<TKey, TValue> d)
    {
        return RTHelper.CreateDictionary(d.Keys, d.Values);
    }
    public static SerializableDictionary<TKey, TValue> ToKVPList<TKey, TValue>(this IDictionary<TKey, TValue> d)
    {
        return new SerializableDictionary<TKey, TValue>(d.Keys.ToList(), d.Values.ToList());
    }
}

public static class RTHelper
{
    /// <summary>
    /// Creates a dictionary out of the passed keys and values
    /// </summary>
    public static Dictionary<T, U> CreateDictionary<T, U>(IEnumerable<T> keys, IList<U> values)
    {
        return keys.Select((k, i) => new { k, v = values[i] }).ToDictionary(x => x.k, x => x.v);
    }
}

public static class IListExtensions
{
    public static bool InBounds<T>(this IList<T> list, int i)
    {
        return i > -1 && i < list.Count;
    }
}
