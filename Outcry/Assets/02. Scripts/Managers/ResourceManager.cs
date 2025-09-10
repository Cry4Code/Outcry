using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Burst.Intrinsics;
using UnityEditor.Tilemaps;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private Dictionary<string, Object> assetPool;

    public T LoadAsset<T>(string assetName,string path) where T : Object
    {
        T result = default;
        string assetPath = Path.Combine(path,assetName);

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

    public async Task<T> LoadAssetAsync<T>(string assetName, string path) where T : Object
    {
        T result = default;
        string assetPath = path + assetName;

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

    public void ClearPool()
    {
        assetPool.Clear();
    }

}
