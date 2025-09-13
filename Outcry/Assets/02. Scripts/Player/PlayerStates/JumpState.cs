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
        if (!player.PlayerMove.IsGrounded() && player.Inputs.Player.Jump.triggered)
            player.ChangeState(new DoubleJumpState());
        else if (player.PlayerMove.IsWallTouched(out var isWallInLeft, out var wallHit))
            player.ChangeState(new WallJumpState());
    }

    public void LogicUpdate(PlayerController player)
    {
        if (player.PlayerMove.IsGrounded()) player.ChangeState(new IdleState());
        else player.PlayerMove.HandleGravity();
    }

    public void Exit(PlayerController player) { }
}
