using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalJumpAttackSubState : AirSubState
{
    public override void Enter(PlayerController player)
    {
        player.PlayerAnimator.OnBoolParam(PlayerAnimID.SubNormalJumpAttack);
    }

    public override void HandleInput(PlayerController player)
    {
        
    }

    public override void LogicUpdate(PlayerController player)
    {
        
    }

    public override void Exit(PlayerController player)
    {
        player.PlayerAnimator.OffBoolParam(PlayerAnimID.SubNormalJumpAttack);
    }
}
