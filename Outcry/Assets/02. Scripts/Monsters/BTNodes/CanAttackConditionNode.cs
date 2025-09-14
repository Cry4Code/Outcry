using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터가 공격 가능한 상태인지 확인하는 노드
/// </summary>
public class CanAttackConditionNode : ConditionNode
{
    private MonsterAIBase monsterAI;
    public CanAttackConditionNode(MonsterAIBase monsterAI)
    {
        this.monsterAI = monsterAI;
    }

    protected override bool IsCondition()
    {
        bool result = false;
        
        if (!monsterAI.IsAttacking) 
            result = true;
        else
            result = false;
        
        Debug.Log($"CanAttackConditionNode is called: {result}");
        return result;
    }
}