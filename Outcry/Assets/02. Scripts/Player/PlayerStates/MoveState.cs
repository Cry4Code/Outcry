using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IPlayerState
{
    public void Enter(PlayerController player) => player.SetAnimation("Move");

    public void HandleInput(PlayerController player)
    {
        var input = player.Inputs.Player.Move.ReadValue<Vector2>();
        if (player.Inputs.Player.Jump.triggered
            && player.PlayerMove.isGrounded
            && !player.PlayerMove.isGroundJump)
        {
            player.ChangeState<JumpState>();
            return;
        }
        if (input.x == 0)
        {
            player.ChangeState<IdleState>();
            return;
        }
        // else if (player.Inputs.Player.Dodge.triggered) player.ChangeState(new DodgeState());

        // 땅에 안닿아있고, 벽에 닿았고, 좌우 입력이 벽의 방향과 같을 때 벽 짚기로 변경
        if (!player.PlayerMove.isGrounded
            && player.PlayerMove.isWallTouched
            && ((input.x < 0 && player.PlayerMove.lastWallIsLeft) || (input.x > 0 && !player.PlayerMove.lastWallIsLeft) ))
        {
            player.ChangeState<WallHoldState>();
            return;
        }


    }


    public void LogicUpdate(PlayerController player)
    {
        player.PlayerMove.Move();
        if (player.PlayerMove.rb.velocity.y < 0)
        {
            player.ChangeState<FallState>();
            return;
        }
        
    }

    public void Exit(PlayerController player) { }
}
