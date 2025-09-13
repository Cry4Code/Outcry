using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHoldState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        Debug.Log("벽짚기 진입");
        player.PlayerMove.ChangeGravity(true);
    }

    public void Exit(PlayerController player)
    {
        Debug.Log("벽짚기 탈출");
    }

    public void HandleInput(PlayerController player)
    {
        var moveInput = player.Inputs.Player.Move.ReadValue<Vector2>();
        if ((((moveInput.x < 0 && player.PlayerMove.lastWallIsLeft) 
            || moveInput.x > 0 && !player.PlayerMove.lastWallIsLeft)) 
            && player.Inputs.Player.Jump.triggered
            && player.PlayerMove.curWall != player.PlayerMove.prevWall)
        {
            player.ChangeState<WallJumpState>();
        }

        if (moveInput.x == 0) player.ChangeState<FallState>();
        else if (player.PlayerMove.isGrounded) player.ChangeState<IdleState>();
        
    }

    public void LogicUpdate(PlayerController player)
    {
        if (player.PlayerMove.keyboardLeft != player.PlayerMove.lastWallIsLeft)
            player.PlayerMove.Move();
    }
}
