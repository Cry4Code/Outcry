using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CompositeNode : Node
{
    [SerializeReference] protected List<Node> children;
    [SerializeField] protected int currentIndex;

    public CompositeNode(List<Node> children = null)
    {
        this.children = children ?? new List<Node>();
        this.currentIndex = 0;
    }

    public override NodeState Tick()
    {
        for (; currentIndex < children.Count; currentIndex++)
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
        }
        currentIndex = 0;
        return NodeState.Success;
    }

    public virtual void AddChild(Node node)
    {
        children.Add(node);
    }
    
    public override void Reset()
    {
        currentIndex = 0;
        foreach (var child in children)
        {
            child.Reset();
        }
    }
}
