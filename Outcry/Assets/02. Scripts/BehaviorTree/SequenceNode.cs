using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[Serializable]
public class SequenceNode : CompositeNode
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

            if (state == NodeState.Failure)
            {
                currentIndex = 0;
                return NodeState.Failure;
            }

            currentIndex++;
        }
        currentIndex = 0;
        return NodeState.Success;
    }
}
