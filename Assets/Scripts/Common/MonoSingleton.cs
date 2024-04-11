using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static bool shuttingDown = false;
    protected static T instance = null;
    private static object locker = new object();
    public static T Instance{
        get{
            if (shuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance " + typeof(T) + " already CHOIed. Returning null.");
                return null;
            }
            lock (locker)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(T)) as T;
                    if (instance == null)
                    {
                        instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    }

                    DontDestroyOnLoad(instance);
                }
            }
            
            return instance;
        }
    }

    public static bool IsNull()
    {
        return instance == null;
    }

    private void Awake(){
        if(instance == null){
            instance = this as T;
        }
        else if(instance != this){
            Debug.LogError("Another instance of " + GetType() + "is already exist. Destroying duplicated one.");
            DestroyImmediate(this);
            return;
        }        
    }

    private void OnDestroy()
    {
        shuttingDown = true;
    }
    private void OnApplicationQuit()
    {
        shuttingDown = true;
    }
}
