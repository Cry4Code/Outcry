using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#region 임시 클래스들

//임시 클래스들
public class SkillNode
{
    public int skillId;
    public SequenceNode skillNode;
}
public static class BehaviorTreeNodeData
{
    public static List<SkillNode> skillNodes = new List<SkillNode>()
    {
        new SkillNode() { skillId = 1, skillNode = new Skill1Sequence("skillId: 1") },
        new SkillNode() { skillId = 2, skillNode = new Skill1Sequence("skillId: 2") },
        new SkillNode() { skillId = 3, skillNode = new Skill1Sequence("skillId: 3") },
        new SkillNode() { skillId = 4, skillNode = new Skill1Sequence("skillId: 4") },
        new SkillNode() { skillId = 5, skillNode = new Skill1Sequence("skillId: 5") }
    };
}

public class Skill1Sequence : SequenceNode
{
    public Skill1Sequence(string nodeName)
    {
        this.nodeName = nodeName;
    }

    public override NodeState Tick()
    {
        Debug.Log(nodeName);
        return base.Tick();
    }
}

//이상 임시끝
#endregion

// [RequireComponent(typeof(BossMonster))] //todo. think. 쓸까 말까?
public class BossMonsterAI : MonsterAIBase
{
    public override void InitializeBehaviorTree()
    {
        SelectorNode rootNode = new SelectorNode();
        
        //isDead
        ConditionNode isDeadNode = new ConditionNode(() => false); //임시
        rootNode.AddChild(isDeadNode);
        
        //AttackSequence
        SequenceNode attackSequenceNode = new SequenceNode();
        ConditionNode canAttackNode = new ConditionNode(() => true); //임시
        SelectorNode attackSelectorNode = new SelectorNode();
        ActionNode waitActionNode = new ActionNode(() => NodeState.Running); //대기 액션 노드 임시
        attackSequenceNode.AddChild(canAttackNode);
        attackSequenceNode.AddChild(attackSelectorNode);
        attackSequenceNode.AddChild(waitActionNode);
        
        rootNode.AddChild(attackSequenceNode);
        
        BossMonsterModel monsterModel = (BossMonsterModel)monster.MonsterData;
        if (monsterModel == null)
        {
            Debug.Log("monsterModel 이게 null이라서 짜증나겠지만 어쨋든 null인걸 어쩌라고.. 짜증나......");
        }
        
        //스페셜 스킬 셀럭터 노드 자식들 생성.
        SelectorNode specialSkillSelectorNode = new SelectorNode();
        foreach (int id in monsterModel.specialSkillIds )
        {
            SkillNode skillNode = BehaviorTreeNodeData.skillNodes.Find(x => x.skillId == id);
            
            if (skillNode != null)
            {
                specialSkillSelectorNode.AddChild(skillNode.skillNode);
            }
        }
        attackSelectorNode.AddChild(specialSkillSelectorNode);
        
        //일반 스킬 셀럭터 노드 자식들 생성.
        SelectorNode commonSkillSelectorNode = new SelectorNode();
        foreach (int id in monsterModel.commonSkillIds)
        {
            SkillNode skillNode = BehaviorTreeNodeData.skillNodes.Find(x => x.skillId == id);
            if (skillNode != null)
            {
                commonSkillSelectorNode.AddChild(skillNode.skillNode);
            }
        }
        attackSelectorNode.AddChild(commonSkillSelectorNode);
        
        //chase
        SelectorNode chaseSelectorNode = new SelectorNode();
        
        //dash
        SequenceNode dashSequenceNode = new SequenceNode();
        ConditionNode canDashNode = new ConditionNode(() => false); //임시
        ActionNode dashActionNode = new ActionNode(() => NodeState.Running); //임시
        dashSequenceNode.AddChild(canDashNode);
        dashSequenceNode.AddChild(dashActionNode);
        chaseSelectorNode.AddChild(dashSequenceNode);
        
        //chase Action
        ActionNode chaseActionNode = new ActionNode(() => NodeState.Running); //임시
        chaseSelectorNode.AddChild(chaseActionNode);
        
        this.rootNode = rootNode;
        Debug.Log("rootNode initialized");
    }
}
