using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownAttackSubState : GroundSubState
{
    public override void Enter(PlayerController controller)
    {
        controller.Animator.OnBoolParam(PlayerAnimID.SubDownAttack);
    }

    public override void HandleInput(PlayerController player)
    {
        
    }

    public override void LogicUpdate(PlayerController player)
    {
        
    }

    public override void Exit(PlayerController player)
    {
        player.Animator.OffBoolParam(PlayerAnimID.SubDownAttack);
    }
}
