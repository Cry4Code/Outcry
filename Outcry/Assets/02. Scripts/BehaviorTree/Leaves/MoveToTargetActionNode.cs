using System;
using UnityEngine;

public class MoveToTargetActionNode : ActionNode
{
    private Transform transform;
    private Transform target;
    private float speed;
    private float stoppingDistance;

    public MoveToTargetActionNode(Transform transform, Transform target, float speed, float stoppingDistance,
        Func<NodeState> action = null)
    {
        this.transform = transform;
        this.target = target;
        this.speed = speed;
        this.stoppingDistance = stoppingDistance;

        this.action = action ?? MoveToTarget;
    }

    private NodeState MoveToTarget()
    {
        Debug.Log("MoveToTarget");
        if (target == null)
        {
            return NodeState.Failure;
        }

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= stoppingDistance)
        {
            return NodeState.Success;
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
        }
        return NodeState.Failure;
    }
}
