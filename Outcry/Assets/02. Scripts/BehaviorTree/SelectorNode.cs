using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SelectorNode : CompositeNode
{
    public override NodeState Tick()
    {
        while(currentIndex < children.Count)
        {
            NodeState state = children[currentIndex].Tick();
            if (state == NodeState.Running)
            {
                return NodeState.Running;
            }
            if (state == NodeState.Success)
            {
                currentIndex = 0;
                return NodeState.Success;
            }
            currentIndex++;
        }
        currentIndex = 0;
        return NodeState.Failure;
    }
}
