using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    private Dictionary<string, SceneBase> scenes;
    private SceneBase currentScene;
    private string managerSceneName = "InitManagerScene";

    private AsyncOperation loadingSceneAsync;

    public static event Action OnSceneActivationComplete;

    protected override void Awake()
    {
        base.Awake();

        scenes = new Dictionary<string, SceneBase>
        {
            { "TitleScene", null },
            { "LobbyScene", null },
            { "LoadingScene", null },
            { "StageScene", null },
        };
    }

    public void RegisterCurrentScene(SceneBase sceneBase)
    {
        if (currentScene != null)
        {
            currentScene.SceneExit();
        }
        currentScene = sceneBase;
    }

    public async void LoadScene(string sceneName)
    {
        if (!scenes.ContainsKey(sceneName))
        {
            Debug.LogWarning($"{sceneName} 씬이 존재하지 않습니다.");
            return;
        }

        if (currentScene != null && currentScene.gameObject.scene.name == sceneName)
        {
            return;
        }

        if (currentScene != null)
        {
            currentScene.SceneExit();
        }

        // currentScene을 여기서 null로 설정하여 이전 씬의 참조 즉시 해제
        currentScene = null;

        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            await Task.Yield();
        }

        // [수정] FindObjectOfType 대신 이벤트를 방송합니다.
        OnSceneActivationComplete?.Invoke();
    }

    public void UnloadScene(string sceneName)
    {
        if (!scenes.ContainsKey(sceneName))
        {
            Debug.LogWarning($"{sceneName} 씬이 존재하지 않습니다.");
            return;
        }

        if (currentScene != null)
        {
            currentScene.SceneExit();
            currentScene = null;
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }

    public async void LoadInitManager()
    {
        var op = SceneManager.LoadSceneAsync(managerSceneName, LoadSceneMode.Additive);
        while (!op.isDone)
        {
            await Task.Yield();
        }
        SceneManager.UnloadSceneAsync(managerSceneName);
    }

    public string GetSceneName()
    {
        return currentScene.SceneName;
    }

    /// <summary>
    /// LoadingScreenController가 호출할 씬 로드 기능. StageScene에서만 StageManager 생성
    /// </summary>
    public async Task LoadScenesAsync(string mainScene, List<string> additiveScenes)
    {
        loadingSceneAsync = SceneManager.LoadSceneAsync(mainScene, LoadSceneMode.Single);
        loadingSceneAsync.allowSceneActivation = false;

        if (additiveScenes != null)
        {
            foreach (var sceneName in additiveScenes)
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
        }

        while (loadingSceneAsync.progress < 0.9f)
        {
            await Task.Yield();
        }
    }

    /// <summary>
    /// LoadingScreenController가 모든 로딩 완료 후 호출
    /// </summary>
    public void ActivateLoadedScenes()
    {
        if (loadingSceneAsync != null)
        {
            loadingSceneAsync.allowSceneActivation = true;
            StartCoroutine(OnSceneActivated()); // 씬 활성화 후처리 코루틴 시작
        }
    }

    /// <summary>
    /// 씬이 활성화된 후 SceneBase의 진입점들 호출
    /// </summary>
    private IEnumerator OnSceneActivated()
    {
        // 씬이 완전히 활성화될 때까지 대기
        while (!loadingSceneAsync.isDone)
        {
            yield return null;
        }

        OnSceneActivationComplete?.Invoke();
    }
}
