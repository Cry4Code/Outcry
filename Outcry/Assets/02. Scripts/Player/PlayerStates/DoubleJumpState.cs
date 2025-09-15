using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.SetAnimation(PlayerAnimID.DoubleJump, true);
        player.PlayerMove.DoubleJump();
    }

    public void HandleInput(PlayerController player) 
    {
        if(player.PlayerMove.isWallTouched)
        {
            player.ChangeState<WallHoldState>();
            return;
        }
    }

    public void LogicUpdate(PlayerController player)
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

    public void Exit(PlayerController player) { }
}
