using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionNode : LeafNode
{
    protected Func<bool> condition;

    /// <summary>
    /// new 생성자를 통해 ConditionNode를 생성할때는 condition을 넣어줘야합니다.
    /// 자식클래스를 생성할때는 IsCondition을 오버라이드하고 condition을 넣어주지 않아도 됩니다.
    /// </summary>
    /// <param name="condition"></param>
    public ConditionNode(Func<bool> condition = null)
    {
        if (condition == null)
        {
            this.condition = IsCondition;
        }
        else
        {
            this.condition = condition;
        }
    }

    /// <summary>
    /// ConditionNode를 상속받는 자식 클래스에서 오버라이드
    /// ConditionNode를 생성할 때 Func<bool> condition을 넣어주지 않으면 이 함수가 기본으로 사용됨
    /// base.IsCondition() 호출 하면 true 리턴이므로 주의할 것
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsCondition()
    {
        return true;
    }

    public override NodeState Tick()
    {
        if (condition.Invoke())
        {
            return NodeState.Success;
        }
        else
        {
            return NodeState.Failure;
        }
    }
}
