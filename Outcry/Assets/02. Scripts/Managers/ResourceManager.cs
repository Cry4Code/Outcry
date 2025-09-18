using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

// --------------- ResourceManager의 자체 참조 카운트가 필요한 이유 ---------------
// Addressables의 참조 카운트는 API 호출 횟수만을 기록합니다.
// ResourceManager는 여러 시스템(StageManager, UIManager 등)의 요청을 받아
// 에셋 하나당 Addressables.Load를 대표로 단 한 번만 호출합니다.
// 따라서 ResourceManager가 자체적으로 자신에게 요청한 고객이 몇 명인지를 세지 않으면
// 한 고객이 Unload를 요청했을 때 다른 고객이 여전히 사용 중임에도 불구하고
// 에셋을 조기에 메모리에서 해제하는 심각한 문제가 발생할 수 있습니다.
// 이 참조 카운트는 그 문제를 해결하기 위한 고객 관리 장부입니다.

public class ResourceManager : Singleton<ResourceManager>
{
    // 로드한 에셋 관리
    private Dictionary<string, Object> assetPool = new Dictionary<string, Object>();

    // Addressable로 로드한 에셋 핸들 관리(메모리 해제용)
    private Dictionary<string, AsyncOperationHandle> addressableHandles = new Dictionary<string, AsyncOperationHandle>();
    // 참조 카운트 관리
    private Dictionary<string, int> refCounts = new Dictionary<string, int>();

    // 동기 Resources 에셋 로드
    public T LoadAsset<T>(string assetName,string path) where T : Object
    {
        T result = default;

        string assetPath = Path.Combine(path, assetName);

        if (!assetPool.ContainsKey(assetPath))
        {
            var asset =  Resources.Load<T>(assetPath);
            if (asset == null) 
            {
                Debug.LogWarning($"{assetPath} 를 불러오기에 실패했습니다.");
                return default(T);
            }

            assetPool.Add(assetPath, asset);
        }

        result = (T)assetPool[assetPath];

        return result;
    }

    // 비동기 Resources 에셋 로드
    public async Task<T> LoadAssetAsync<T>(string assetName, string path) where T : Object
    {
        T result = default;

        string assetPath = Path.Combine(path, assetName);

        if (!assetPool.ContainsKey(assetPath))
        {
            var op = Resources.LoadAsync<T>(assetPath);
            while (!op.isDone)
            {
                await Task.Yield();
            }

            var obj = op.asset;
            if (obj == null)
            {
                Debug.LogWarning($"{assetPath} 를 불러오기에 실패했습니다.");
                return default(T);
            }

            assetPool.Add(assetPath, obj);
        }

        result = (T)assetPool[assetPath];

        return result;
    }

    #region 어드레서블 로드/언로드
    // 비동기 어드레서블 에셋 로드
    // 이미 로드된 에셋은 캐시에서 즉시 반환하며 참조 카운트 1 증가
    public async Task<T> LoadAssetAddressableAsync<T>(string address) where T : Object
    {
        // 이미 로드된 에셋인지 확인 (참조 카운트 확인)
        if (refCounts.TryGetValue(address, out int count))
        {
            refCounts[address]++; // 참조 카운트 1 증가

            // 로딩은 완료되었는지 핸들을 통해 확인
            if (addressableHandles.TryGetValue(address, out var handle) && handle.IsDone)
            {
                return assetPool[address] as T;
            }
            else // 아직 로딩 중인 경우 완료될 때까지 대기
            {
                await addressableHandles[address].Task;
                return assetPool[address] as T;
            }
        }

        // 처음 로드하는 에셋인 경우
        refCounts[address] = 1; // 참조 카운트를 1로 초기화

        var loadHandle = Addressables.LoadAssetAsync<T>(address);
        addressableHandles[address] = loadHandle; // 핸들 저장

        await loadHandle.Task; // 로드가 끝날 때까지 대기

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            assetPool[address] = loadHandle.Result; // 캐시에 저장
            return loadHandle.Result;
        }
        else
        {
            Debug.LogError($"[ResourceManager] 에셋 로드 실패: {address}");
            refCounts.Remove(address); // 실패 시 참조 카운트 정보 제거
            addressableHandles.Remove(address);
            Addressables.Release(loadHandle); // 실패한 핸들은 즉시 릴리즈
            return null;
        }
    }

    // 사용이 끝난 어드레서블 에셋 참조 카운트 1 감소
    // 참조 카운트가 0이 되면 실제 메모리에서 언로드
    public void UnloadAddressableAsset(string address)
    {
        if (!refCounts.ContainsKey(address))
        {
            Debug.LogWarning($"[ResourceManager] 언로드하려는 에셋이 로드된 적 없습니다: {address}");
            return;
        }

        refCounts[address]--; // 참조 카운트 1 감소

        // 참조 카운트가 0 이하가 되면
        if (refCounts[address] <= 0)
        {
            refCounts.Remove(address); // 참조 카운트 제거
            assetPool.Remove(address); // 풀에서 제거

            if (addressableHandles.TryGetValue(address, out AsyncOperationHandle handle))
            {
                Addressables.Release(handle); // 메모리 해제
                addressableHandles.Remove(address); // 핸들 딕셔너리에서 제거
            }
        }
    }

    // 동기 Addressable 에셋 로드
    // 이미 로드된 에셋은 캐시에서 즉시 반환하며 참조 카운트 1 증가
    /// <summary>
    /// 메인 스레드를 차단하여 게임이 멈출 수 있으므로 사용을 권장하지 않음
    /// </summary>
    public T LoadAssetAddressable<T>(string address) where T : Object
    {
        // 이미 로드되었거나 로딩 중인 에셋인지 확인
        if (refCounts.TryGetValue(address, out _))
        {
            refCounts[address]++; // 참조 카운트 1 증가
            // 로드가 완료될 때까지 기다렸다가 캐시에서 반환
            return addressableHandles[address].WaitForCompletion() as T;
        }

        // 처음 로드하는 에셋인 경우
        refCounts[address] = 1; // 참조 카운트를 1로 초기화

        var loadHandle = Addressables.LoadAssetAsync<T>(address);
        addressableHandles[address] = loadHandle; // 핸들 저장

        // 작업이 완료될 때까지 메인 스레드를 멈추고 대기
        T result = loadHandle.WaitForCompletion();

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            assetPool[address] = result; // 캐시에 저장
            return result;
        }
        else
        {
            Debug.LogError($"[ResourceManager] 동기 로드 실패: {address}");
            // 실패 시 모든 정보 제거
            refCounts.Remove(address);
            addressableHandles.Remove(address);
            Addressables.Release(loadHandle);
            return null;
        }
    }
    #endregion

    public void ClearResourcePools()
    {
        foreach(var handle in addressableHandles.Values)
        {
            Addressables.Release(handle);
        }

        assetPool.Clear();
        addressableHandles.Clear();

        // Resources 폴더에서 로드된 에셋 중 더 이상 참조되지 않는 것을 언로드
        Resources.UnloadUnusedAssets();
    }

    private void OnDestroy()
    {
        ClearResourcePools();
    }
}
