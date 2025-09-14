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
        SelectorNode attackSelectorNode = new SelectorNode();
        // SelectorNode delaySelectorNode = new SelectorNode();    //공격 애니메이션이 끝나면 딜레이를 진행하도록 돕는 셀렉터
        
        // InverterNode inverterNode = new InverterNode();
        // IsAttackingConditionNode isAttackingConditionNode = new IsAttackingConditionNode(monster.Animator);
        WaitActionNode waitActionNode = new WaitActionNode(3.0f); //대기 액션 노드 임시

        attackSequenceNode.AddChild(attackSelectorNode);
        // attackSequenceNode.AddChild(delaySelectorNode);
        
        attackSequenceNode.AddChild(waitActionNode);
        
        
        rootNode.AddChild(attackSequenceNode);

        // IsInAttackRangeConditionNode isInAttackRangeNode = new IsInAttackRangeConditionNode(
        //     monster.transform, target.transform, monster.MonsterData.attackRange);
        
        // attackSequenceNode.AddChild(isInAttackRangeNode);

        #region ForDebug

        attackSelectorNode.AddChild(new ActionNode(() =>
        {
            Debug.Log("BossMonster Attack!");
            monster.Animator.Play("Attack");    //Trigger를 쓰니까 전환되는 동안 애니메이션 전환되는 프레임마다 BT가 한두번 찝져서 발동함.
            return NodeState.Success;
        })); //공격 액션 노드 임시

        #endregion
        
        
        //스킬은 보스몬스터로 형변환 후에 접근.
        BossMonsterModel monsterModel = (BossMonsterModel)monster.MonsterData;
        if (monsterModel == null)
        {
            Debug.Log("monsterModel 이게 null이라서 짜증나겠지만 어쨋든 null인걸 어쩌라고.. 짜증나......");
        }
        
        // // 스페셜 스킬 셀럭터 노드 자식들 생성.
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
        
        //todo. movetotargetActionNode는 테스트용으로 작성한 것이므로, 추후에 chase랑 patrol로 나누어서 작성해야됨.
        MoveToTargetActionNode moveToTargetActionNode = new MoveToTargetActionNode(monster.transform, target.transform, monster.MonsterData.chaseSpeed, monster.MonsterData.attackRange);
        rootNode.AddChild(moveToTargetActionNode);

        #region NamingForDebug

        rootNode.nodeName = "RootNode";
        attackSequenceNode.nodeName = "AttackSequenceNode";
        inverterNode.nodeName = "InverterNode";
        isAttackingConditionNode.nodeName = "IsAttackingConditionNode";
        // isInAttackRangeNode.nodeName = "CanAttackConditionNode";
        attackSelectorNode.nodeName = "AttackSelectorNode";
        waitActionNode.nodeName = "WaitActionNode";
        moveToTargetActionNode.nodeName = "MoveToTargetActionNode";
        
        #endregion
        
        this.rootNode = rootNode;
        Debug.Log("rootNode initialized");
    }
}
