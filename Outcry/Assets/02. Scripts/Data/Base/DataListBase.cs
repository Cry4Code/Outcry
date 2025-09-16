using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 데이터 보관 및 반환 담당
/// </summary>
[Serializable]
public abstract class DataListBase<T>
{
    [SerializeField]
    protected List<T> dataList;
    public List<T> DataList { get; private set; }   //현재는 간소하게 List로만! 하지만 런타임 메모리 절약을 위해 딕셔너리와 병행하여 사용하는 것을 고려할 것.
    
    // todo. think 필요없을듯?
    public abstract void Initialize();

    public void AddToList(T data)
    {
        dataList.Add(data);
    }
}
