using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperSlashSequenceNode : SkillSequenceNode
{
    [SerializeField] private float elapsedTime = 0f;
    private bool skillTriggered = false;
    private int animationHash = AnimatorStrings.MonsterParameter.UpperSlash;  
    
    protected override bool CanPerform()
    {
        bool result;
        bool isInRange;
        bool isCooldownComplete;

        // 플레이어와 거리 2m 이내에 있을때
        // MonsterSkillModel 수정 필요 (Stomp 스킬 참조)
        if (Vector2.Distance(monster.transform.position, target.transform.position) <= skillData.range)
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

        // 기본 피해 : HP 2칸 감소
        // 넉백 추가는 미정

        // ** 플레이어 대응 **
        //      - 회피 사용 가능
        //      - 패링 사용 가능

        if (!skillTriggered)
        {
            monster.Animator.SetTrigger(animationHash);

            // todo. 플레이어 데미지 처리
            monster.AttackController.SetDamage(skillData.damage1);

            skillTriggered = true;
        }

        // 시작 직후 Running 강제
        elapsedTime += Time.deltaTime;
        if (elapsedTime < 0.1f)
        {            
            return NodeState.Running;
        }

        bool isSkillAnimationPlaying = IsSkillAnimationPlaying(AnimatorStrings.MonsterAnimation.UpperSlash);
        if (isSkillAnimationPlaying)
        {
            Debug.Log($"Running skill: {skillData.skillName} (ID: {skillData.skillId})");
            state = NodeState.Running;
        }
        else
        {
            Debug.Log($"Skill End: {skillData.skillName} (ID: {skillData.skillId})");

            monster.AttackController.SetDamage(0);  // 데미지 초기화
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
