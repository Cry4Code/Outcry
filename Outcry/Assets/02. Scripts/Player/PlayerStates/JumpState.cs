using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.SetAnimation("Jump");
        player.PlayerMove.Jump();
    }

    public void HandleInput(PlayerController player)
    {
        if (player.Inputs.Player.Jump.triggered)
        {
            if(player.PlayerMove.isGroundJump && !player.PlayerMove.isDoubleJump)
            {
                player.ChangeState<DoubleJumpState>();

            }
        }
        else if (player.PlayerMove.IsWallTouched(out var isWallInLeft, out var wallHit))
            player.ChangeState<WallJumpState>();
    }

    public void LogicUpdate(PlayerController player)
    {
        player.PlayerMove.Move();
        if (player.PlayerMove.isGrounded) player.ChangeState<IdleState>();
    }

    public void Exit(PlayerController player) { }
}
