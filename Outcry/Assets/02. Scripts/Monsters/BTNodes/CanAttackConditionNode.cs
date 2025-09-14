using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanAttackConditionNode : ConditionNode
{
    private Transform me;
    private Transform target;
    private float attackRange;
    
    public CanAttackConditionNode(Transform me, Transform target, float attackRange)
    {
        this.me = me;
        this.target = target;
        this.attackRange = attackRange;
    }
    protected override bool IsCondition()
    {
        if (Vector2.Distance(me.position, target.position) <= attackRange)
            return true;
        else
            return false;
    }
}
