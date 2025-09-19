using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class  Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    [Tooltip("True로 설정하면 씬이 바뀌어도 파괴되지 않습니다.")]
    [SerializeField] private bool isPersistent = false;

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

    protected virtual void Awake()
    {
        if (instance == null)
        {
            // 아직 static 인스턴스가 할당되지 않았다면 현재 인스턴스 할당
            instance = this as T;

            // isPersistent 플래그가 true일 경우 씬이 변경되어도 파괴되지 않음
            if (isPersistent)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (instance != this)
        {
            // 이미 인스턴스가 존재하고 그 인스턴스가 자신이 아니라면 중복이므로 현재 게임 오브젝트 파괴
            // Instance 프로퍼티를 통해 접근했을 때 이미 다른 인스턴스가 생성되었을 수 있음
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        // 파괴되는 객체가 현재 싱글톤 인스턴스인 경우 static 참조를 null로 설정
        // 이를 통해 isPersistent가 false일 때 씬이 바뀌면 다음 씬에서 새로운 인스턴스를 찾을 수 있음
        if (instance == this)
        {
            instance = null;
        }
    }

    private static T Create()
    {
        GameObject go = new GameObject(typeof(T).Name);
        T result = go.AddComponent<T>();
        
        return result;
    }
}
