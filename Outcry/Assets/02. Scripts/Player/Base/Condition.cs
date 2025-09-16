using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Condition
{
    private string observerKey;
    Observable<int> curValue;
    public int startValue;
    public int maxValue;
    public int passiveValue; // 주기적으로 변하는 값

    public void Init(string key)
    {
        observerKey = key;
        curValue = new Observable<int>(observerKey, startValue);
    }
    public float GetPercent()
    {
        return curValue.Value / maxValue;
    }
    public void Add(int value)
    {
        // 최대 값을 넘지 않도록 제한
        curValue.Value = Mathf.Min(curValue.Value + value, maxValue);
    }

    public void Substract(int value)
    {
        // 최소 값보다 작아지지 않도록 제한
        curValue.Value = Mathf.Max(curValue.Value - value, 0);
    }

    public float CurValue()
    {
        return curValue.Value;
    }
}
