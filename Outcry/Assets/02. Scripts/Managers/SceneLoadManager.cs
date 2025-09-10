using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    private Dictionary<string, SceneBase> scenes;
    private SceneBase currentScene;
    private string managerSceneName = "ManagerScene";

    public async void LoadScene(string sceneName)
    {
        if (!scenes.ContainsKey(sceneName))
        {
            Debug.LogWarning($"{sceneName} 씬이 존재하지 않습니다.");
            return;
        }
        if (currentScene == scenes[sceneName])
        {
            return;
        }
        if (currentScene != null)
        {
            currentScene.SceneExit();
        }
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            await Task.Yield();
        }

        currentScene = scenes[sceneName];
        currentScene.SceneAwake();
        currentScene.SceneEnter();

    }
    /*
    public async void UnloadScene(string sceneName)
    {
        if (!scenes.ContainsKey(sceneName))
        {
            Debug.LogWarning($"{sceneName} 씬이 존재하지 않습니다.");
            return;
        }
        if (currentScene != null)
        {
            currentScene.SceneExit();
        }
    }
    */
    public async void LoadManager()
    {
        var op = SceneManager.LoadSceneAsync(managerSceneName,LoadSceneMode.Additive);
        while (!op.isDone)
        {
            await Task.Yield();
        }
        SceneManager.UnloadSceneAsync(managerSceneName);
    }
    public string GetSceneName()
    {
        return currentScene.name;
    }

}
