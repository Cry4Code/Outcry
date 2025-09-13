using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.SetAnimation("WallJump");
        player.PlayerMove.WallJump();
    }

    public void HandleInput(PlayerController player) { }

    public void LogicUpdate(PlayerController player)
    {
        if (player.PlayerMove.isGrounded) player.ChangeState<IdleState>();
    }

    public void Exit(PlayerController player) { }
}
