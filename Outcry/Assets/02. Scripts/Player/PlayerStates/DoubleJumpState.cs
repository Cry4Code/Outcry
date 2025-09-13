using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.SetAnimation("DoubleJump");
        player.PlayerMove.DoubleJump();
    }

    public void HandleInput(PlayerController player) 
    {
        if(player.PlayerMove.isWallTouched)
        {
            player.ChangeState<WallHoldState>();
        }
    }

    public void LogicUpdate(PlayerController player)
    {
        player.PlayerMove.Move();
        if (player.PlayerMove.rb.velocity.y < 0) player.ChangeState<FallState>();
        else if (player.PlayerMove.isGrounded) player.ChangeState<IdleState>();
    }

    public void Exit(PlayerController player) { }
}
