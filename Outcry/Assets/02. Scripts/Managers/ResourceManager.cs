using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : Singleton<ResourceManager>
{
    // 로드한 에셋 관리
    private Dictionary<string, Object> assetPool = new Dictionary<string, Object>();

    // Addressable로 로드한 에셋 핸들 관리(메모리 해제용)
    private Dictionary<string, AsyncOperationHandle> addressableHandles = new Dictionary<string, AsyncOperationHandle>();

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

    // 동기 Addressable 에셋 로드
    public T LoadAssetAddressable<T>(string address) where T : Object
    {
        // 캐시 확인 (이미 로드했다면 바로 반환)
        if (assetPool.TryGetValue(address, out Object asset))
        {
            if (addressableHandles.ContainsKey(address) && addressableHandles[address].IsValid())
            {
                return asset as T;
            }
        }

        // 비동기 작업을 시작하고 핸들을 받음
        var handle = Addressables.LoadAssetAsync<T>(address);

        // 작업이 완료될 때까지 메인 스레드를 차단하고 대기
        T result = handle.WaitForCompletion();

        // 결과 처리
        if (handle.Status == AsyncOperationStatus.Succeeded && result != null)
        {
            // 성공 시 캐시 및 핸들 딕셔너리에 추가
            assetPool.Add(address, result);
            addressableHandles.Add(address, handle);
        }
        else
        {
            Debug.LogError($"[ResourceManager] 동기 로드 실패: {address}");
            // 실패 시에는 핸들을 즉시 릴리즈하여 메모리 누수 방지
            Addressables.Release(handle);
            return default(T);
        }

        return result;
    }

    // 비동기 Addressable 에셋 로드
    public async Task<T> LoadAssetAddressableAsync<T>(string address) where T : Object
    {
        T result = default;

        // 에셋이 풀에 존재하지 않는 경우
        if (!assetPool.ContainsKey(address))
        {
            // AsyncOperationHandle은 어드레서블 시스템에서 실행되는 하나의 비동기 작업에 대한 모든 정보를 담고 있음
            // 호출하는 순간 비동기 작업에 대한 모든 것을 추적하고 제어
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(address);
            await handle.Task; // 비동기 작업이 끝날 때까지 대기

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                assetPool.Add(address, handle.Result);
                addressableHandles.Add(address, handle);
            }
            else
            {
                Debug.LogWarning($"{address} 를 불러오기에 실패했습니다.");
                return default(T);
            }
        }

        // 에셋이 풀에 이미 존재하는 경우
        if(addressableHandles.ContainsKey(address) && addressableHandles[address].IsValid())
        {
            result = (T)assetPool[address];
        }

        return result;
    }

    public void ReleaseAddressableAsset(string address)
    {
        if(addressableHandles.TryGetValue(address, out AsyncOperationHandle handle))
        {
            // 풀에서 제거
            assetPool.Remove(address);

            // 메모리 해제
            Addressables.Release(handle);

            // 핸들 딕셔너리에서 제거
            addressableHandles.Remove(address);
        }
        else
        {
            Debug.LogWarning($"릴리즈할 에셋을 찾을 수 없습니다. 주소: {address}");
        }
    }

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
