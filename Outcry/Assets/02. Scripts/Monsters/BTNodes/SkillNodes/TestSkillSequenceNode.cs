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

    protected override bool SkillConditionCheck()
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

    protected override NodeState SkillUse()
    {
        NodeState state;

        if (false) //어떤 이유든 스킬을 사용할 수 없는 조건
        {
            return NodeState.Failure;
        }
        
        //todo. 스킬 사용 로직 구현.
        if (!skillTriggered)
        {
            monster.Animator.SetTrigger("Skill");
            skillTriggered = true;
        }
        
        //스킬 애니메이션이 끝났는지 확인.
        if (elapsedTime < 0.1f && !monster.Animator.GetCurrentAnimatorStateInfo(0).IsName("Skill"))
        {
            // 스킬 애니메이션이 끝난 후 //0.1f는 애니메이션이 트리거되는 짧은 프레임 방지. 
            Debug.Log($"Using skill: {skillData.skillName} (ID: {skillData.skillId})");
            skillTriggered = false;
            state = NodeState.Success;
        }
        else
        {
            Debug.Log($"Running skill: {skillData.skillName} (ID: {skillData.skillId})");
            state = NodeState.Running;
        }
        
        return state;
    }
}
