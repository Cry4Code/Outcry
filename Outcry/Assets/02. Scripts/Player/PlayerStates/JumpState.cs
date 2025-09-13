using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.SetAnimation("Jump");
        player.PlayerMove.Jump();
        if (!player.PlayerMove.isGroundJump) player.PlayerMove.isGroundJump = true;
    }

    public void HandleInput(PlayerController player)
    {
        if (player.Inputs.Player.Jump.triggered)
        {
            if(!player.PlayerMove.isDoubleJump)
            {
                player.ChangeState<DoubleJumpState>();

            }
        }
        else if (player.PlayerMove.isWallTouched)
            player.ChangeState<WallHoldState>();
    }

    public void LogicUpdate(PlayerController player)
    {
        if (!player.PlayerMove.isGroundJump) player.PlayerMove.isGroundJump = true;
        player.PlayerMove.Move();
        if (player.PlayerMove.rb.velocity.y < 0) player.ChangeState<FallState>();
    }

    public void Exit(PlayerController player) { }
}
