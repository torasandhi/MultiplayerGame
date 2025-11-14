using PurrNet;
using UnityEngine;

/// Use as: public class MyManager : Singleton<MyManager> { ... }
[DefaultExecutionOrder(-100)]
public abstract class NetworkSingleton<T> : NetworkBehaviour where T : NetworkSingleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"Duplicate Singleton '{typeof(T).Name}' found. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        _instance = (T)this;
        DontDestroyOnLoad(gameObject);
    }
}

