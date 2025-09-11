using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    protected List<Node> children;

    protected CompositeNode(List<Node> children = null)
    {
        this.children = children;
    }

    public virtual void Add(Node node)
    {
        children.Add(node);
    }
}
