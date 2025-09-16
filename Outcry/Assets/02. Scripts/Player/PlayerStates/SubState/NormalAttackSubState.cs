using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackSubState : GroundSubState
{
    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        player.PlayerAnimator.OnBoolParam(PlayerAnimID.SubNormalAttack);
    }

    public override void HandleInput(PlayerController player)
    {
        
    }

    public override void LogicUpdate(PlayerController player)
    {
        
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
        player.PlayerAnimator.OffBoolParam(PlayerAnimID.SubNormalAttack);
    }
}
