using System;

[Serializable]
public abstract class DecoratorNode : Node
{ 
    protected Node child;

    public virtual void SetChild(Node child)
    {
        this.child = child;
    }

    public override void Reset()
    {
        child?.Reset();
    }
}
