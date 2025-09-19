using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownAttackState : DownAttackSubState
{
    public override void Enter(PlayerController controller)
    {
        base.Enter(controller);
        controller.Animator.ClearBool();
        controller.isLookLocked = true; 
        controller.Condition.canStaminaRecovery.Value = false;
        controller.Animator.SetTriggerAnimation(PlayerAnimID.DownAttack);
        controller.Inputs.Player.Move.Disable();
        controller.Move.rb.gravityScale = 3.5f;
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
        if (player.Move.isGrounded)
        {
            player.ChangeState<IdleState>();
            return;
        }
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
        player.Move.rb.gravityScale = 1f;
    }
}
