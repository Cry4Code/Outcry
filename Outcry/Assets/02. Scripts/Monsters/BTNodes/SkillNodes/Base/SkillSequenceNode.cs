using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스킬 시퀀스를 만들때마다 상속 받으면 됩니다.
/// </summary>
[Serializable]
public abstract class SkillSequenceNode : SequenceNode
{
    protected int skillId;
    protected MonsterBase monster;
    protected Player target;
    protected MonsterSkillModel skillData;
    
    public virtual void InitializeSkillSequenceNode(MonsterBase monster, Player target, MonsterSkillModel skillData)
    {
        this.skillId = skillData.skillId;
        this.monster = monster;
        this.target = target;
        this.skillData = skillData;
        
        ConditionNode canPerform = new ConditionNode(CanPerform);
        ActionNode skillAction = new ActionNode(SkillAction);
        
        
        //노드 이름 설정 (디버깅용)
        canPerform.nodeName = "CanPerform";
        skillAction.nodeName = "SkillAction";
        
        children.Clear();
        AddChild(canPerform);
        AddChild(skillAction);
    }
    protected abstract bool CanPerform();

    protected abstract NodeState SkillAction();


}
