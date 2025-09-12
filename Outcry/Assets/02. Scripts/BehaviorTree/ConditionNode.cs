using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionNode : LeafNode
{
    protected Func<bool> condition;

    public ConditionNode(Func<bool> condition = null)
    {
        this.condition = condition;
    }

    public override NodeState Tick()
    {
        if (condition.Invoke())
        {
            return NodeState.Success;
        }
        else
        {
            return NodeState.Failure;
        }
    }
}
