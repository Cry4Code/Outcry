using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;

    public void UpdateProgress(float progress)
    {
        if (loadingBar != null)
        {
            loadingBar.value = progress;
        }
    }
}
