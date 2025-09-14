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
        foreach (var child in children)
        {
            switch (child.Tick())
            {                
                case NodeState.Running:
                    return NodeState.Running;
                case NodeState.Failure:
                    return NodeState.Failure;
                case NodeState.Success:
                    continue;
            }
        }

        return NodeState.Success;
    }
}
