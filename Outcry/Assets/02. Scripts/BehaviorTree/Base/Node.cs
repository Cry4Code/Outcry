using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    Running,
    Success,
    Failure
}
[Serializable]
public abstract class Node
{
    protected NodeState nodeState;
    protected string nodeName;  // 디버그용

    public abstract NodeState Tick();

    public virtual void Reset()
    {

    }
}