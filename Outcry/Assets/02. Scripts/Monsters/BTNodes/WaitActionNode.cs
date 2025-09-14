using UnityEngine;

public class WaitActionNode : ActionNode
{
    private float waitTime;
    private float elapsedTime;
    
    public WaitActionNode(float waitTime)
    {
        this.waitTime = waitTime;
        this.elapsedTime = 0f;
    }

    protected override NodeState Act()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= waitTime)
        {
            return NodeState.Success;
        }

        return NodeState.Running;
    }

    public override void Reset()
    {
        elapsedTime = 0f;
    }
}
