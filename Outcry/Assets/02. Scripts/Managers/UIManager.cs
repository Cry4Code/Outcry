using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<string, UIBase> uis;
    public static int screenWidth = 1920;
    public static int screenHeight = 1080;

   public T Show<T>()
    {
        string uiName = typeof(T).Name;
        return default(T);
    }
}
