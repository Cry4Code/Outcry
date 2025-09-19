using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalJumpAttackSubState : AirSubState
{
    public override void Enter(PlayerController controller)
    {
        controller.Animator.OnBoolParam(PlayerAnimID.SubNormalJumpAttack);
    }

    public override void HandleInput(PlayerController player)
    {
        
    }

    public override void LogicUpdate(PlayerController player)
    {
        
    }

    public override void Exit(PlayerController player)
    {
        player.Animator.OffBoolParam(PlayerAnimID.SubNormalJumpAttack);
    }
}
