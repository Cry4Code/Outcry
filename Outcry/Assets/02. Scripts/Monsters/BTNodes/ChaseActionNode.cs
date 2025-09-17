using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animator 추가하기 위해 MoveToTargetActionNode 상속 받음.
/// </summary>
public class ChaseActionNode : MoveToTargetActionNode
{
    private Animator animator;
    public ChaseActionNode(Transform me, Transform target, float speed, float stoppingDistance, Animator animator) : base(me, target, speed, stoppingDistance)
    {
        this.animator = animator;
    }
    
    protected override NodeState Act()
    {
        NodeState state = base.Act();
        //todo. Animation 추가되면 수정하기.
        if (state == NodeState.Running)
        {
            animator.SetBool(AnimatorStrings.MonsterParameter.Running, true);
        }
        else
        {
            animator.SetBool(AnimatorStrings.MonsterParameter.Running, false);
        }
        return state;
    }
}
