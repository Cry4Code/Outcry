using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : LeafNode
{
    protected Func<NodeState> action;

    /// <summary>
    /// new 생성자를 통해 ActionNode 생성할때는 action을 넣어줘야합니다.
    /// 자식클래스를 생성할때는 Act 오버라이드하고 action을 넣어주지 않아도 됩니다.
    /// </summary>
    /// <param name="action"></param>
    public ActionNode(Func<NodeState> action = null)
    {
        if (action == null)
        {
            this.action = Act;
        }
        else
        {
            this.action = action;
        }
    }

    /// <summary>
    /// ActionNode 상속받는 자식 클래스에서 오버라이드
    /// ActionNode 생성할 때 Func<NodeState> action 넣어주지 않으면 이 함수가 기본으로 사용됨
    /// base.Act() 호출 하면 NodeState.Success 리턴이므로 주의할 것
    /// </summary>
    /// <returns></returns>
    protected virtual NodeState Act()
    {
        return NodeState.Success;
    }

    public override NodeState Tick()
    {
        return action.Invoke();
    }
}
