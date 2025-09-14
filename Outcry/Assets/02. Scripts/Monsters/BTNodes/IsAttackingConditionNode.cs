using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 범용성을 위해서 얘를 써먹으려고 했으나, 그냥 각 액션마다 애니메이션 체크하는게 나을듯
/// 공격 애니메이션이 재생중인지 확인하는 노드
/// </summary>
public class IsAttackingConditionNode : ConditionNode
{
    private Animator animator;
    private float attackStartTime = -1f;
    private const float delay = 0.1f;
    public IsAttackingConditionNode(Animator animator)
    {
        this.animator = animator;
    }

    protected override bool IsCondition()
    {
        bool result = false;
        
        bool isAttackingAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        bool isTransitioning = animator.IsInTransition(0);
        bool isNextAnimationAttacking = animator.GetNextAnimatorStateInfo(0).IsName("Attack");
        
        if (isAttackingAnimation || (isTransitioning && isNextAnimationAttacking))
        {
            result = true;
        }
        Debug.Log($"IsAttackingConditionNode is called: {result}");
        return result;
    }
}