using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class StompSkillSequenceNode : SkillSequenceNode
{
    public StompSkillSequenceNode(int skillId) : base(skillId)
    {
        this.nodeName = "StompSkillSequenceNode";
    }

    protected override bool CanPerform()
    {
        bool result;
        bool isInRange;
        bool isCooldownComplete;
        
        //플레이어와의 거리 2m 이내에 있을때
        //todo. 2f는 2m 이내. MonsterSkillModel에서 이걸 받아올 수 있도록 변경해야함
        if (Vector2.Distance(monster.transform.position, target.transform.position) <= skillData.range)
        {
            isInRange = true;
        }
        else
        {
            isInRange = false;
        }

        //쿨다운 확인
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= skillData.cooldown)
        {
            isCooldownComplete = true;
        }
        else
        {
            isCooldownComplete = false;
        }

        result = isInRange && isCooldownComplete;
        Debug.Log($"Skill {skillData.skillName} used? {result} : {elapsedTime} / {skillData.cooldown}");
        return result;
    }

    protected override NodeState SkillAction()
    {
        NodeState state;
        //기본 피해 : HP 2칸 감소
        //추가 효과 : 피격시 플레이어 다운

        // - **플레이어 대응**
        //     - 회피 사용 가능
        //     - 패링 사용 가능

        if (!skillTriggered)
        {
            elapsedTime = 0f;
            FlipCharacter();
            monster.Animator.SetTrigger(AnimatorStrings.MonsterParameter.Stomp);
            //todo. player damage 처리
            monster.AttackController.SetDamages(skillData.damage1);
            skillTriggered = true;
        }

        elapsedTime += Time.deltaTime;
        
        if (elapsedTime < 0.1f) //시작 직후는 무조건 Running
        {
            return NodeState.Running;
        }
        
        bool isSkillAnimationPlaying = IsSkillAnimationPlaying(AnimatorStrings.MonsterAnimation.Stomp);
        if (isSkillAnimationPlaying)
        {
            Debug.Log($"Running skill: {skillData.skillName} (ID: {skillData.skillId})");
            state = NodeState.Running;
        }
        else
        {
            Debug.Log($"Skill End: {skillData.skillName} (ID: {skillData.skillId})");
            
            monster.AttackController.SetDamages(0); //데미지 초기화.
            skillTriggered = false;
            state = NodeState.Success;
        }

        return state;
    }
}
