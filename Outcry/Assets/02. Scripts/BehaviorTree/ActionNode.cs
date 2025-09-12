using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : LeafNode
{
    protected Func<NodeState> action;

    public ActionNode(Func<NodeState> action = null)
    {
        this.action = action;
    }

    public override NodeState Tick()
    {
        return action.Invoke();
    }
}
