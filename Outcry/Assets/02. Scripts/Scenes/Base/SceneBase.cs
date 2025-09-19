using UnityEngine;

/// <summary>
/// 씬 이름과 씬 베이스를 상속받는 클래스의 이름을 같게 해주세요.
/// </summary>
public abstract class SceneBase : MonoBehaviour
{
    public string SceneName { get; protected set; }

    public abstract void SceneAwake();
    public abstract void SceneEnter();
    public abstract void SceneExit();

    protected void OnEnable()
    {
        SceneLoadManager.OnSceneActivationComplete += HandleSceneReady;
    }

    protected void OnDisable()
    {
        SceneLoadManager.OnSceneActivationComplete -= HandleSceneReady;
    }

    protected void HandleSceneReady()
    {
        Debug.Log("씬 활성화 완료 방송 수신. 초기화 시작.");

        // SceneBase의 핵심 로직을 여기서 실행
        SceneAwake();
        SceneEnter();
    }
}
