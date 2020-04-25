using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabCache : MonoBehaviour
{
    public GameObject[] PrefabsToCache;
    private Dictionary<string, Object> _resourceCache = new Dictionary<string, Object>();

    public static PrefabCache GetCache()
    {
        PrefabCache instance = FindObjectOfType<PrefabCache>();
        return instance;
    }

    void Awake()
    {
        foreach (var obj in PrefabsToCache)
        {
            _resourceCache.Add(obj.name, obj);
        }
    }

    public T Load<T>(string path) where T : Object
    {
        Object obj;
        string[] dirs = path.Split('/');
        string objName = dirs[dirs.Length-1];
        if (_resourceCache.TryGetValue(objName, out obj))
        {            
            return (T)obj;
        }
        return null;
    }
}
