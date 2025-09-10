using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<string, UIBase> uis;
    public static int screenWidth = 1920;
    public static int screenHeight = 1080;

    public T Show<T>() where T : UIBase
    {
        string uiName = typeof(T).Name;
        string path = Path.Combine(Paths.Prefabs.UI, uiName);

        uis.TryGetValue(uiName, out UIBase ui);
        
        // uis에 ui가 없으면 새로 로드
        if (ui == null)
        {
            ui = Load<T>();
            uis.Add(uiName, ui);
        }

        ui.Open();

        return (T)ui;
    }

    public void Hide<T>()
    {
        string uiName = typeof(T).Name;

        uis.TryGetValue(uiName, out UIBase ui);

        // uis에 ui가 없으면, 경고
        if (ui == null)
        {
            Debug.LogWarning($"{uiName} 이 없습니다.");
            return;
        }

        ui.Close();
    }

    public T GetUI<T>() where T : UIBase
    {
        string uiName = typeof(T).Name;

        uis.TryGetValue(uiName, out UIBase ui);

        if (ui == null)
        {
            Debug.LogWarning($"{uiName} 이 없습니다.");
            return default;
        }

        return (T)ui;
    }

    private T Load<T>() where T : UIBase
    {
        // 캔버스 생성
        GameObject parentCanvas = new GameObject(typeof(T).Name + " Canvas");
        
        var canvas = parentCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var canvasScaler = parentCanvas.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2 (screenWidth, screenHeight);

        parentCanvas.AddComponent<GraphicRaycaster>();

        // ui 프리팹 로드
        string uiName = typeof(T).Name;
        string path = Path.Combine(Paths.Prefabs.UI, uiName);

        var ui = ResourceManager.Instance.LoadAsset<T>(uiName, path);

        // ui 생성
        var go = Instantiate(ui, parentCanvas.transform);
        go.name = go.name.Replace("(Clone)", "");

        var result = go.GetComponent<UIBase>();
        result.canvas = canvas;
        result.canvas.sortingOrder = uis.Count;

        return (T)result;
    }
}
