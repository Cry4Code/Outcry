using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompSkillSequenceNode : SkillSequenceNode
{
    private float elapsedTime = 0f;
    private bool skillTriggered = false;
    private string animationName = AnimatorStrings.MonsterParameter.Stomp;
    
    protected override bool CanPerform()
    {
        bool result;
        bool isInRange;
        bool isCooldownComplete;
        
        //플레이어와의 거리 2m 이내에 있을때
        //todo. 2f는 2m 이내. MonsterSkillModel에서 이걸 받아올 수 있도록 변경해야함
        if (Vector2.Distance(monster.transform.position, target.transform.position) <= 2f)
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
        //기본 피해 : HP 2칸 감소
        //추가 효과 : 피격시 플레이어 다운

        // - **플레이어 대응**
        //     - 회피 사용 가능
        //     - 패링 사용 가능

        if (!skillTriggered)
        {
            monster.Animator.SetTrigger(animationName);
            //todo. player damage 처리
            
            
            
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
            skillTriggered = false;
            state = NodeState.Success;
        }

        return state;
    }

    //todo. 나중에 부모 클래스에 넣어두고 상속해서 사용하도록 해도 될듯.
    private bool IsSkillAnimationPlaying(string animationName)
    {
        //스킬 애니메이션이 끝났는지 확인.
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
