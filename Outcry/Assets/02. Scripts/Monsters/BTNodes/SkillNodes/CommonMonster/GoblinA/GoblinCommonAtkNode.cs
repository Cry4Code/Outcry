using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinCommonAtkNode : SkillSequenceNode
{

    // 모든 고블린이 공유해도 되는 노드인가?
    public GoblinCommonAtkNode(int skillId) : base(skillId)
    {

    }

    protected override bool CanPerform()
    {
        return default;
    }

    // 엑션 클립 이름, 파라미터 이름을 통일하면 중복 사용가능하지 않을까?
    protected override NodeState SkillAction()
    {
        return default;
    }
}
