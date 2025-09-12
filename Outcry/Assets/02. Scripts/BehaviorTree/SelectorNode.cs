using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : CompositeNode
{
    public override NodeState Tick()
    {
        foreach (var child in children)
        {
            switch (child.Tick())
            {
                case NodeState.Success:
                    return NodeState.Success;
                case NodeState.Running:
                    return NodeState.Running;
                case NodeState.Failure:
                    continue;
            }
        }

        return NodeState.Failure;
    }
}
