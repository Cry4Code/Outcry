using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsInAttackRangeConditionNode : ConditionNode
{
    private Transform me;
    private Transform target;
    private float attackRange;
    
    public IsInAttackRangeConditionNode(Transform me, Transform target, float attackRange)
    {
        this.me = me;
        this.target = target;
        this.attackRange = attackRange;
    }
    protected override bool IsCondition()
    {
        bool result;
        if (Vector2.Distance(me.position, target.position) <= attackRange)
            result = true;
        else
            result = false;
        Debug.Log($"CanAttackConditionNode is called: {result}");
        return result;
    }
}
