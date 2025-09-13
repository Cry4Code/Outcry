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
            return;
        }

    }

    public void LogicUpdate(PlayerController player)
    {
        //if (player.Inputs.Player.Move.ReadValue<Vector2>().x != 0)
        //{
        //    player.PlayerMove.Move();
        //    return;
        //}
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
    }

    public void Exit(PlayerController player) { }
}
