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
    public void ShuffleChildren()   //todo. 랜덤 셔플 알고리즘은 추후 한번 더 체크해봐야됨. (임시생성)
    {
        for (int i = 0; i < children.Count; i++)
        {
            Node temp = children[i];
            int randomIndex = UnityEngine.Random.Range(i, children.Count);
            children[i] = children[randomIndex];
            children[randomIndex] = temp;
        }
    }
}
