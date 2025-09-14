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
        NodeState result;
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= waitTime)
        {
            result = NodeState.Success;
        }
        else
        {
            {result = NodeState.Running;}
        }
        
        Debug.Log($"WaitActionNode is called: {result} ({elapsedTime}/{waitTime})");
        return result;
    }

    public override void Reset()
    {
        elapsedTime = 0f;
    }
}
