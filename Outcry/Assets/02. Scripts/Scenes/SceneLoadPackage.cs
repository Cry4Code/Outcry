using System.Collections.Generic;

/// <summary>
/// LoadingScene이 수행해야 할 작업의 명세를 담는 데이터 클래스
/// </summary>
public class SceneLoadPackage
{
    public string MainSceneName { get; private set; }
    public List<string> AdditiveSceneNames { get; private set; } = new List<string>();
    public List<string> ResourceAddressesToLoad { get; private set; } = new List<string>();

    public SceneLoadPackage(string mainSceneName)
    {
        MainSceneName = mainSceneName;
    }
}