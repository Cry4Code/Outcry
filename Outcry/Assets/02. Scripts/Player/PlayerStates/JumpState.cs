using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.SetAnimation(PlayerAnimID.Jump, true);
        player.isLookLocked = true; 
        player.PlayerMove.Jump();
        if (!player.PlayerMove.isGroundJump) player.PlayerMove.isGroundJump = true;
    }

    public void HandleInput(PlayerController player)
    {
        var moveInput = player.Inputs.Player.Move.ReadValue<Vector2>();
        
        if (player.Inputs.Player.Jump.triggered)
        {
            if(!player.PlayerMove.isDoubleJump)
            {
                player.ChangeState<DoubleJumpState>();
                return;
            }
        }
        if (player.PlayerMove.isWallTouched)
        {
            player.ChangeState<WallHoldState>();
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

    public void LogicUpdate(PlayerController player)
    {
        if (!player.PlayerMove.isGroundJump)
        {
            player.PlayerMove.isGroundJump = true;
            return;
        }
        player.PlayerMove.Move();
        if (player.PlayerMove.rb.velocity.y < 0)
        {
            player.ChangeState<FallState>();
            return;
        }
    }

    public void Exit(PlayerController player)
    {
        player.isLookLocked = false;
    }
}
