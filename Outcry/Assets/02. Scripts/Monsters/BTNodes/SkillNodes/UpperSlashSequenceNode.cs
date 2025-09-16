using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperSlashSequenceNode : SkillSequenceNode
{
    private float elapsedTime = 0f;
    private bool skillTriggered = false;
    // MonsterParameter에 UpperSlash 추가 후 수정
    private string animationName = AnimatorStrings.MonsterParameter.Stomp;  
    
    protected override bool CanPerform()
    {
        bool result;
        bool isInRange;
        bool isCooldownComplete;

        // 플레이어와 거리 2m 이내에 있을때
        // MonsterSkillModel 수정 필요 (Stomp 스킬 참조)
        if (Vector2.Distance(monster.transform.position, target.transform.position) <= 2f)
        {
            isInRange = true;
        }
        else
        {
            isInRange = false;
        }

        // 쿨다운 체크
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= skillData.cooldown)
        {
            isCooldownComplete = true;
            elapsedTime = 0f;
        }
        else
        {
            isCooldownComplete = false;
        }

        result = isInRange && isCooldownComplete;
        Debug.Log($"Skill used? {result} : {elapsedTime} / {skillData.cooldown}");
        return result;
    }

    protected override NodeState SkillAction()
    {
        NodeState state;

        // 기본 피해 2칸
        // 넉백 추가는 미정
        // 플레이어 회피, 패링 가능

        if (!skillTriggered)
        {
            monster.Animator.SetTrigger(animationName);
            // todo. 플레이어 데미지 처리

            skillTriggered = true;
        }

        // 시작 직후 Running 강제
        elapsedTime += Time.deltaTime;
        if (elapsedTime < 0.1f)
        {            
            return NodeState.Running;
        }

        bool isSkillAnimationPlaying = IsSkillAnimationPlaying(AnimatorStrings.MonsterAnimation.Stomp); // MonsterAnimation에 UpperSlash 추가 후 수정
        if (isSkillAnimationPlaying)
        {
            Debug.Log($"Running skill: {skillData.skillName} (ID: {skillData.skillId})");
            state = NodeState.Running;
        }
        else
        {
            Debug.Log($"Skill End: {skillData.skillName} (ID: {skillData.skillId})");
            skillTriggered = false;
            state = NodeState.Success;
        }

        return state;
    }

    // 나중에 부모클래스에 넣고 상속받는다고 함. Stomp 스킬 참조
    private bool IsSkillAnimationPlaying(string animationName)
    {
        //스킬 애니메이션이 끝났는지 확인
        bool isSkillAnimationPlaying = monster.Animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);

        if (isSkillAnimationPlaying)
        {
            Debug.Log($"Running skill: {skillData.skillName} (ID: {skillData.skillId})");
            return true;
        }
        else
        {
            Debug.Log($"Using skill: {skillData.skillName} (ID: {skillData.skillId})");
            skillTriggered = false;
            return false;
        }
    }
}
