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
        
        //AttackSequence
        SequenceNode attackSequenceNode = new SequenceNode();
        CanAttackConditionNode canAttackConditionNode = new CanAttackConditionNode(this);
        SelectorNode attackSelectorNode = new SelectorNode();
        WaitActionNode waitActionNode = new WaitActionNode(3.0f); //대기 액션 노드 임시

        attackSequenceNode.AddChild(canAttackConditionNode);
        attackSequenceNode.AddChild(attackSelectorNode);
        attackSequenceNode.AddChild(waitActionNode);
        
        rootNode.AddChild(attackSequenceNode);
        
        // #region ForDebug
        //
        // attackSelectorNode.AddChild(new ActionNode(() =>
        // {
        //     Debug.Log("BossMonster Attack!");
        //     monster.Animator.SetTrigger("Attack");
        //     IsAttacking = true;
        //     return NodeState.Success;
        // })); //공격 액션 노드 임시
        //
        // #endregion
        
        
        //스킬은 보스몬스터로 형변환 후에 접근.
        BossMonsterModel monsterModel = (BossMonsterModel)monster.MonsterData;
        if (monsterModel == null)
        {
            Debug.Log("monsterModel 이게 null이라서 짜증나겠지만 어쨋든 null인걸 어쩌라고.. 짜증나......");
        }
        
        // 스페셜 스킬 셀럭터 노드 자식들 생성.
        SelectorNode specialSkillSelectorNode = new SelectorNode();
        specialSkillSelectorNode.nodeName = "SpecialSkillSelectorNode"; //디버깅용 노드 이름 설정.
        foreach (int id in monsterModel.specialSkillIds )
        {
            SkillSequenceNode skillNode = SkillNodeDatabase.GetSkillNode(id);
            MonsterSkillModel skillData = Temp_DataBase.GetMonsterSkillById(id);
            if (skillNode != null)
            {
                skillNode.InitializeSkillSequenceNode(monster, target, skillData);
                skillNode.nodeName = "SkillNode_" + skillData.skillName; //디버깅용 노드 이름 설정.
                specialSkillSelectorNode.AddChild(skillNode);
            }
        }
        specialSkillSelectorNode.ShuffleChildren();
        
        attackSelectorNode.AddChild(specialSkillSelectorNode);
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
        
        //ChaseSelector
        SelectorNode chaseSelectorNode = new SelectorNode();
        //todo. 추후 DashSequenceNode및, ActionNode 추가할 것.
        ChaseActionNode chaseActionNode = new ChaseActionNode(
            monster.transform, target.transform, monster.MonsterData.chaseSpeed, monster.MonsterData.attackRange,
            monster.Animator);
        chaseSelectorNode.AddChild(chaseActionNode);
        
        rootNode.AddChild(chaseSelectorNode);

        #region NamingForDebug

        rootNode.nodeName = "RootNode";
        attackSequenceNode.nodeName = "AttackSequenceNode";
        // inverterNode.nodeName = "InverterNode";
        // isAttackingConditionNode.nodeName = "IsAttackingConditionNode";
        canAttackConditionNode.nodeName = "CanAttackConditionNode";
        attackSelectorNode.nodeName = "AttackSelectorNode";
        waitActionNode.nodeName = "WaitActionNode";
        chaseSelectorNode.nodeName = "ChaseSelectorNode";
        chaseActionNode.nodeName = "ChaseActionNode";
        
        #endregion
        
        this.rootNode = rootNode;
        Debug.Log("rootNode initialized");
    }
}
