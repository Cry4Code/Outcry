using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumbleOfRuinSkillSequenceNode : SkillSequenceNode
{
    public RumbleOfRuinSkillSequenceNode(int skillId) : base(skillId)
    {
        this.nodeName = "RumbleOfRuinSkillSequenceNode";
    }

    protected override bool CanPerform()
    {
        bool result;
        bool isCooldownComplete;
        bool isLowHealth;

        if (skillTriggered) //한번 실행되면 다시 실행될 일 없음. 실행 끝난 이후 리셋 X
        {
            return false;
        }
        
        //체력이 일정 이하일때
        isLowHealth = monster.Condition.CurrentHealth < skillData.triggerHealth * monster.Condition.MaxHealth;

        //쿨다운 확인
        isCooldownComplete = lastUsedTime - Time.time <= skillData.cooldown;
        

        result = isLowHealth || isCooldownComplete;
        Debug.Log($"Skill {skillData.skillName} used? {result} : {Time.time - lastUsedTime} / {skillData.cooldown}");
        return result;
    }

    protected override NodeState SkillAction()
    {
        NodeState state;

        if (skillTriggered)
        {
            //이미 실행된적 있으면 실행하지 않음.
            return NodeState.Failure;
        }

        lastUsedTime = Time.time;
        FlipCharacter();
        //todo. 몬스터 애니메이션 // monster.Animator.SetTrigger();
        skillTriggered = true;
        
        if (Time.time - lastUsedTime < 0.1f) //시작 직후는 무조건 Running
        {
            return NodeState.Running;
        }
        
        //스킬 애니메이션 적절할때 데미지 주기

        bool isSkillAnimationPlaying = Random.Range(0, 2) == 0; //= IsSkillAnimationPlaying()
        if (isSkillAnimationPlaying)
        {
            Debug.Log($"Running skill: {skillData.skillName} (ID: {skillData.skillId})");
            state = NodeState.Running;
        }
        else
        {
            Debug.Log($"Skill End: {skillData.skillName} (ID: {skillData.skillId})");
            state = NodeState.Success;
        }
        
        return state;
    }
}
