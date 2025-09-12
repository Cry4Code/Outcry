using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static Paths;

public class GameManager : Singleton<GameManager>
{
    private void Start()
    {
        var testPrefab = ResourceManager.Instance.LoadAssetAddressableSync<GameObject>("JoannaStatusUI/TestPrefab.prefab");

        if(testPrefab != null)
        {
            GameObject prefab = Instantiate(testPrefab, Vector3.zero, Quaternion.identity);
            StartCoroutine(DestroyTestCoroutine(prefab));
        }
    }

    private IEnumerator DestroyTestCoroutine(GameObject prefab)
    {
        yield return new WaitForSeconds(5f);

        Destroy(prefab);

        ResourceManager.Instance.ReleaseAddressableAsset("JoannaStatusUI/TestPrefab.prefab");
    }
}
