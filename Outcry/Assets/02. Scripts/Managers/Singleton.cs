using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class  Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get 
        {
            if (instance == null)
            {
                T[] objects = FindObjectsByType<T>(FindObjectsSortMode.None) as T[];
                if (objects.Length > 0)
                {
                    instance = objects[0];
                    for (int i = 1; i < objects.Length; i++)
                    { 
                        DestroyImmediate(objects[i].gameObject); //매니저가 다음프레임까지 남아있지 않게 하기 위해서 DestroyImmediate 를 사용
                    }
                }
                else
                {
                    instance = Create();
                }
            }
            return instance;
        }
    }
    private static T Create()
    {
        GameObject go = new GameObject(typeof(T).Name);
        T result = go.AddComponent<T>();
        
        return result;
    }
}
