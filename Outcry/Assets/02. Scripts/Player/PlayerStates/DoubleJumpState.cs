using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpState : AirSubState
{
    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        player.SetAnimation(PlayerAnimID.DoubleJump, true);
        player.PlayerMove.DoubleJump();
    }
    
    public override void HandleInput(PlayerController player) 
    {
        var moveInput = player.Inputs.Player.Move.ReadValue<Vector2>();
        
        if(player.PlayerMove.isWallTouched 
           && ((moveInput.x < 0 && player.PlayerMove.lastWallIsLeft) || (moveInput.x > 0 && !player.PlayerMove.lastWallIsLeft)))
        {
            player.ChangeState<WallHoldState>();
            return;
        }
    }

    public override void LogicUpdate(PlayerController player)
    {
        var moveInput = player.Inputs.Player.Move.ReadValue<Vector2>();
        if(moveInput != null)
            player.PlayerMove.Move();

        if (player.PlayerMove.rb.velocity.y < 0)
        {
            player.ChangeState<FallState>();
            return;
        }
        if (player.PlayerMove.isGrounded)
        {
            player.ChangeState<IdleState>();
            return;
        }
        
        if (player.Inputs.Player.NormalAttack.triggered && moveInput.y < 0)
        {
            player.isLookLocked = true;
            player.ChangeState<DownAttackState>();
            return;
        }
        
        if (player.Inputs.Player.NormalAttack.triggered && !player.PlayerAttack.HasJumpAttack)
        {
            player.isLookLocked = true;
            player.ChangeState<NormalJumpAttackState>();
            return;
        }
        
        
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
    }
}
