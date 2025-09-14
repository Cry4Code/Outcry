using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

// [RequireComponent(typeof(BossMonster))] //todo. think. 쓸까 말까?
public class BossMonsterAI : MonsterAIBase
{
    protected override void InitializeBehaviorTree()
    {
        SelectorNode rootNode = new SelectorNode();
        
        // //isDead
        // ConditionNode isDeadNode = new ConditionNode(() => false); //임시
        // rootNode.AddChild(isDeadNode);
        //
        // //AttackSequence
        // SequenceNode attackSequenceNode = new SequenceNode();
        // ConditionNode canAttackNode = new ConditionNode(() => true); //임시
        // SelectorNode attackSelectorNode = new SelectorNode();
        // ActionNode waitActionNode = new ActionNode(() => NodeState.Running); //대기 액션 노드 임시
        // attackSequenceNode.AddChild(canAttackNode);
        // attackSequenceNode.AddChild(attackSelectorNode);
        // attackSequenceNode.AddChild(waitActionNode);
        //
        // rootNode.AddChild(attackSequenceNode);
        //
        // BossMonsterModel monsterModel = (BossMonsterModel)monster.MonsterData;
        // if (monsterModel == null)
        // {
        //     Debug.Log("monsterModel 이게 null이라서 짜증나겠지만 어쨋든 null인걸 어쩌라고.. 짜증나......");
        // }
        //
        // //스페셜 스킬 셀럭터 노드 자식들 생성.
        // SelectorNode specialSkillSelectorNode = new SelectorNode();
        // foreach (int id in monsterModel.specialSkillIds )
        // {
        //     SkillNode skillNode = BehaviorTreeNodeData.skillNodes.Find(x => x.skillId == id);
        //     
        //     if (skillNode != null)
        //     {
        //         specialSkillSelectorNode.AddChild(skillNode.skillNode);
        //     }
        // }
        // attackSelectorNode.AddChild(specialSkillSelectorNode);
        //
        // //일반 스킬 셀럭터 노드 자식들 생성.
        // SelectorNode commonSkillSelectorNode = new SelectorNode();
        // foreach (int id in monsterModel.commonSkillIds)
        // {
        //     SkillNode skillNode = BehaviorTreeNodeData.skillNodes.Find(x => x.skillId == id);
        //     if (skillNode != null)
        //     {
        //         commonSkillSelectorNode.AddChild(skillNode.skillNode);
        //     }
        // }
        // attackSelectorNode.AddChild(commonSkillSelectorNode);
        //
        // //chase
        // SelectorNode chaseSelectorNode = new SelectorNode();
        //
        // //dash
        // SequenceNode dashSequenceNode = new SequenceNode();
        // ConditionNode canDashNode = new ConditionNode(() => false); //임시
        // ActionNode dashActionNode = new ActionNode(() => NodeState.Running); //임시
        // dashSequenceNode.AddChild(canDashNode);
        // dashSequenceNode.AddChild(dashActionNode);
        // chaseSelectorNode.AddChild(dashSequenceNode);
        //
        // //chase Action
        // ActionNode chaseActionNode = new ActionNode(() => NodeState.Running); //임시
        // chaseSelectorNode.AddChild(chaseActionNode);
        //
        
        MoveToTargetActionNode moveToTargetActionNode = new MoveToTargetActionNode(monster.transform, target.transform, monster.MonsterData.chaseSpeed, monster.MonsterData.attackRange);
        rootNode.AddChild(moveToTargetActionNode);
        this.rootNode = rootNode;
        Debug.Log("rootNode initialized");
    }
}
