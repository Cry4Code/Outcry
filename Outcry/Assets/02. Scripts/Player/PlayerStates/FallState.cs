using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.PlayerMove.ChangeGravity(false);
    }

    public void Exit(PlayerController player)
    {
        
    }

    public void HandleInput(PlayerController player)
    {
        var moveInputs = player.Inputs.Player.Move.ReadValue<Vector2>();

        if (player.PlayerMove.isWallTouched
            && ((player.PlayerMove.lastWallIsLeft && moveInputs.x < 0) || (!player.PlayerMove.lastWallIsLeft && moveInputs.x > 0)))
        {

            player.ChangeState<WallHoldState>();
            return;
        }


        if (player.Inputs.Player.Jump.triggered)
        {
            if(!player.PlayerMove.isGroundJump)
            {
                player.ChangeState<JumpState>();
                return;
            }
            if (!player.PlayerMove.isDoubleJump)
            {
                player.ChangeState<DoubleJumpState>();
                return;
            }
        }
    }

    public void LogicUpdate(PlayerController player)
    {

        // 공중에서 이동 가능
        var input = player.Inputs.Player.Move.ReadValue<Vector2>();
        if (input != null)
        {
            player.PlayerMove.Move();
        }

        if (player.PlayerMove.isGrounded)
        {
            player.ChangeState<IdleState>();
            return;
        }

        
    }
}
