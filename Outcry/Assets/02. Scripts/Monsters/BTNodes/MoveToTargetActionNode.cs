using System;
using UnityEngine;

/// <summary>
/// 기본 이동 노드
/// me가 target을 향해 speed 속도로 이동
/// target과 me의 거리가 stoppingDistance 이내가 되면 성공
/// </summary>
public class MoveToTargetActionNode : ActionNode
{
    private Transform me;
    private Transform target;
    private float speed;
    private float stoppingDistance;

    public MoveToTargetActionNode(Transform me, Transform target, float speed, float stoppingDistance)
    {
        this.me = me;
        this.target = target;
        this.speed = speed;
        this.stoppingDistance = stoppingDistance;
    }

    protected override NodeState Act()
    {
        Debug.Log("MoveToTarget");
        if (target == null)
        {
            return NodeState.Failure;
        }

        Vector2 targetPosition = new Vector2(target.position.x, me.position.y);
        float distance = Vector2.Distance(me.position, targetPosition);
        if (distance <= stoppingDistance)
        {
            return NodeState.Success;
        }
        else
        {
            me.position = Vector2.MoveTowards(
                me.position,
                targetPosition,
                speed * Time.deltaTime
            );
            return NodeState.Running;
        }
    }
}
