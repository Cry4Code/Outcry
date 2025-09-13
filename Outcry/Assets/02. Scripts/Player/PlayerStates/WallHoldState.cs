using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHoldState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        
    }

    public void Exit(PlayerController player)
    {
       
    }

    public void HandleInput(PlayerController player)
    {
        var moveInput = player.Inputs.Player.Move.ReadValue<Vector2>();

        // 벽이 있는 방향으로 입력이 들어왔을 때
        if ((((moveInput.x < 0 && player.PlayerMove.lastWallIsLeft) 
            || moveInput.x > 0 && !player.PlayerMove.lastWallIsLeft)) )
        {
            if(player.Inputs.Player.Jump.triggered
            && player.PlayerMove.curWall != player.PlayerMove.prevWall)
            {
                Debug.Log("벽점으로");
                player.ChangeState<WallJumpState>();
                return;
            }
            if (player.PlayerMove.isWallTouched)
            {
                Debug.Log("중력 감소");
                player.PlayerMove.ChangeGravity(true);
                return;
            }
        }

        if (moveInput.x == 0)
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

    public void LogicUpdate(PlayerController player)
    {
        if (player.PlayerMove.keyboardLeft != player.PlayerMove.lastWallIsLeft)
        {
            player.PlayerMove.Move();
            return;
        }
    }
}
