using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    protected List<Node> children;
    protected int currentIndex;

    public CompositeNode(List<Node> children = null)
    {
        this.children = children ?? new List<Node>();
    }

    public virtual void AddChild(Node node)
    {
        children.Add(node);
    }
}
