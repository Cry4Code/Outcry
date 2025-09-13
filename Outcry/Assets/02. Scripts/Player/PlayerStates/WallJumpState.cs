using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        Debug.Log("벽점!");
        player.SetAnimation("WallJump");
        player.PlayerMove.WallJump();
    }

    public void HandleInput(PlayerController player) 
    {
        if(player.PlayerMove.isWallTouched && player.PlayerMove.prevWall != player.PlayerMove.curWall)
        {

            player.ChangeState<WallHoldState>();
        }

    }

    public void LogicUpdate(PlayerController player)
    {
        player.PlayerMove.Move();
        if(player.PlayerMove.rb.velocity.y < 0) player.ChangeState<FallState>();
        if (player.PlayerMove.isGrounded) player.ChangeState<IdleState>();
    }

    public void Exit(PlayerController player) { }
}
