using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : GroundSubState
{
    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        player.PlayerAnimator.OnBoolParam(PlayerAnimID.Move);

    }

    public override void HandleInput(PlayerController player)
    {
        var input = player.Inputs.Player.Move.ReadValue<Vector2>();
        if (player.Inputs.Player.Jump.triggered
            && player.PlayerMove.isGrounded
            && !player.PlayerMove.isGroundJump)
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
        if (!player.PlayerMove.isGrounded
            && player.PlayerMove.isWallTouched
            && ((input.x < 0 && player.PlayerMove.lastWallIsLeft) || (input.x > 0 && !player.PlayerMove.lastWallIsLeft) ))
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
        if (player.PlayerMove.rb.velocity.y < 0)
        {
            player.ChangeState<FallState>();
            return;
        }
        player.PlayerMove.Move();
        
        
    }

    public override void Exit(PlayerController player) 
    {
        base.Exit(player);
        player.PlayerAnimator.OffBoolParam(PlayerAnimID.Move);
    }
}
