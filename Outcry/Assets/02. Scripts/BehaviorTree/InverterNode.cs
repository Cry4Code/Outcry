using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InverterNode : DecoratorNode
{
    public override NodeState Tick()
    {
        if (child == null)
        {
            return NodeState.Failure;
        }
        NodeState state = child.Tick();
        
        if (state == NodeState.Success)
            return NodeState.Failure;
        else if (state == NodeState.Failure)
            return NodeState.Success;
        else
            return NodeState.Running;
    }
}
