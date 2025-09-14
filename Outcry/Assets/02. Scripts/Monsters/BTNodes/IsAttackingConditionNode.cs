using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsAttackingConditionNode : ConditionNode
{
    private Animator animator;
    
    public IsAttackingConditionNode(Animator animator)
    {
        this.animator = animator;
    }
    
    protected override bool IsCondition()
    {
        bool result = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        Debug.Log($"IsAttackingConditionNode is called: {result}");
        return result;
    }
}