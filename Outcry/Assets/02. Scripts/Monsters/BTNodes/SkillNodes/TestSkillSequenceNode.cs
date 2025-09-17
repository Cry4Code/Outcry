using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 예시로 작성한 임시 클래스
/// </summary>
public class TestSkillSequenceNode : SkillSequenceNode
{
    private float elapsedTime = 0f;
    private bool skillTriggered = false;

    public TestSkillSequenceNode(int skillId) : base(skillId)
    {
    }

    protected override bool CanPerform()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= skillData.cooldown)
        {
            Debug.Log($"Skill Used: {elapsedTime} / {skillData.cooldown}");
            elapsedTime = 0f;
            return true;
        }
        Debug.Log($"Skill cooldown: {elapsedTime} / {skillData.cooldown}");
        return false;
    }

    protected override NodeState SkillAction()
    {
        NodeState state;

        if (false) //어떤 이유든 스킬을 사용할 수 없는 조건
        {
            return NodeState.Failure;
        }
        
        //todo. 스킬 사용 로직 구현.
        if (!skillTriggered)
        {
            monster.Animator.SetTrigger(AnimatorStrings.MonsterParameter.NormalAttack);
            skillTriggered = true;
        }
        
        elapsedTime += Time.deltaTime;
        
        if (elapsedTime < 0.1f) //시작 직후는 무조건 Running
        {
            return NodeState.Running;
        }
        
        //스킬 애니메이션이 끝났는지 확인.
        bool isSkillAnimationPlaying = monster.Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimatorStrings.MonsterParameter.NormalAttack);
        
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
}
