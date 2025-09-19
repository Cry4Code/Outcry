using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : GroundSubState
{
    public override void Enter(PlayerController controller)
    {
        base.Enter(controller);
        controller.Animator.OnBoolParam(PlayerAnimID.Move);
        controller.Condition.canStaminaRecovery.Value = true;

    }

    public override void HandleInput(PlayerController player)
    {
        var input = player.Inputs.Player.Move.ReadValue<Vector2>();
        if (player.Inputs.Player.Jump.triggered
            && player.Move.isGrounded
            && !player.Move.isGroundJump)
        {
            player.ChangeState<JumpState>();
            return;
        }
        if (input.x == 0)
        {
            player.ChangeState<IdleState>();
            return;
        }
        // else if (player.Inputs.Player.Dodge.triggered) player.ChangeState(new DodgeState());

        // 땅에 안닿아있고, 벽에 닿았고, 좌우 입력이 벽의 방향과 같을 때 벽 짚기로 변경
        if (!player.Move.isGrounded
            && player.Move.isWallTouched
            && ((input.x < 0 && player.Move.lastWallIsLeft) || (input.x > 0 && !player.Move.lastWallIsLeft) ))
        {
            player.ChangeState<WallHoldState>();
            return;
        }
        
        if (player.Inputs.Player.NormalAttack.triggered)
        {
            player.isLookLocked = true;
            player.ChangeState<NormalAttackState>();
            return;
        }
        
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
        if (player.Move.rb.velocity.y < 0)
        {
            player.ChangeState<FallState>();
            return;
        }
        player.Move.Move();
        
        
    }

    public override void Exit(PlayerController player) 
    {
        base.Exit(player);
        player.Animator.OffBoolParam(PlayerAnimID.Move);
    }
}
