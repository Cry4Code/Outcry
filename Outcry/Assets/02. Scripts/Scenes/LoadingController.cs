using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    public async void LoadResources()
    {
        var package = GameManager.Instance.NextLoadPackage;
        if (package == null)
        {
            Debug.LogError("로드할 씬 패키지가 없습니다!");
            return;
        }

        // 수행해야 할 Task들을 담을 리스트 생성
        var loadingTasks = new List<Task>();

        // 명세서에 따라 필요한 Task를 리스트에 추가
        // 씬 로드 작업 추가
        loadingTasks.Add(SceneLoadManager.Instance.LoadScenesAsync(package.MainSceneName, package.AdditiveSceneNames));

        // 리소스 로드 작업 추가 (로드할 리소스가 있을 경우에만)
        if (package.ResourceAddressesToLoad.Count > 0)
        {
            loadingTasks.Add(ResourceManager.Instance.LoadAllAssetsAsync(package.ResourceAddressesToLoad));
        }

        // 모든 작업이 끝날 때까지 대기
        await Task.WhenAll(loadingTasks);

        // 모든 로딩이 끝나면 씬 활성화 신호
        SceneLoadManager.Instance.ActivateLoadedScenes();
    }
}