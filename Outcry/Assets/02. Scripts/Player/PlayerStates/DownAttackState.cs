using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownAttackState : DownAttackSubState
{
    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        player.PlayerAnimator.ClearBool();
        player.isLookLocked = true; 
        player.Condition.canStaminaRecovery.Value = false;
        player.PlayerAnimator.SetTriggerAnimation(PlayerAnimID.DownAttack);
        player.Inputs.Player.Move.Disable();
        player.PlayerMove.rb.gravityScale = 3.5f;
    }

    public override void HandleInput(PlayerController player)
    {
        if (player.Inputs.Player.SpecialAttack.triggered)
        {
            player.isLookLocked = false;
            player.ChangeState<SpecialAttackState>();
            return;
        }
        if (player.Inputs.Player.Dodge.triggered)
        {
            player.ChangeState<DodgeState>();
            return;
        }
        if (player.Inputs.Player.Parry.triggered)
        {
            player.ChangeState<StartParryState>();
            return;
        }
    }

    public override void LogicUpdate(PlayerController player)
    {
        if (player.PlayerMove.isGrounded)
        {
            player.ChangeState<IdleState>();
            return;
        }
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
        player.PlayerMove.rb.gravityScale = 1f;
    }
}
