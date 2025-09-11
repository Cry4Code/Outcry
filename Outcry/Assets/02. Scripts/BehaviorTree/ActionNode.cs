using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : Node
{
    Func<NodeState> action;

    protected ActionNode(Func<NodeState> action)
    {
        this.action = action;
    }

    public override NodeState Evaluate()
    {
        return action.Invoke();
    }
}
