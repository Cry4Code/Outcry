using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeSkillSequenceNode : SkillSequenceNode
{
    [SerializeField] private float elapsedTime = 0f;
    private bool skillTriggered = false;

    protected override bool CanPerform()
    {
        bool result;
        bool isInRange;
        bool isCooldownComplete;

        // 플레이어와의 거리 4m 이내에 있을 때
        // todo. MonsterSkillModel에서 이걸 받아오도록 수정, 스킬 Stomp 참고
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
        //기본 피해 : HP 2칸 감소
        //추가 효과 : 오브젝트(Stone) 생성 
        //          - 각 오브젝트는 HP 1칸 감소

        // **플레이어 대응**
        //      - 회피 사용 가능
        //      - 패링 사용 가능


        if (!skillTriggered)
        {            
            monster.Animator.SetTrigger(AnimatorStrings.MonsterParameter.Earthquake);            
            monster.AttackController.SetDamage(skillData.damage1);  // 플레이어 데미지 주기

            skillTriggered = true;
        }

        // 시작 직후 Running 강제
        elapsedTime += Time.deltaTime;
        if (elapsedTime < 0.1f) // 시작 직후는 무조건 Running
        {
            return NodeState.Running;
        }

        // 애니메이션 중 Running 리턴 고정
        bool isSkillAnimationPlaying = IsSkillAnimationPlaying(AnimatorStrings.MonsterAnimation.Earthquake);
        if (isSkillAnimationPlaying)
        {
            Debug.Log($"Running skill: {skillData.skillName} (ID: {skillData.skillId})");
            state = NodeState.Running;
        }
        else
        {
            Debug.Log($"Running skill: {skillData.skillName} (ID: {skillData.skillId})");

            monster.AttackController.SetDamage(0);  //데미지 초기화
            skillTriggered = false;
            state = NodeState.Success;
        }

        return state;
    }

    // 애니메이션 실행 중 확인용 함수
    //todo. 부모 클래스에 넣어서 상속? Stomp 내 주석 확인
    private bool IsSkillAnimationPlaying(string animationName)
    {
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
