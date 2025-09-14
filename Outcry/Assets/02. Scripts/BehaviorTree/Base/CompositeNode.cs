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
