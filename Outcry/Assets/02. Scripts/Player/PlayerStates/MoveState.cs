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
            && !player.PlayerMove.isGroundJump) player.ChangeState<JumpState>();
        else if (input.x == 0) player.ChangeState<IdleState>();
        // else if (player.Inputs.Player.Dodge.triggered) player.ChangeState(new DodgeState());
    }


    public void LogicUpdate(PlayerController player)
    {
        player.PlayerMove.Move();
        if (player.PlayerMove.rb.velocity.y < 0) player.ChangeState<FallState>();
        else if (!player.PlayerMove.isGrounded 
            && player.PlayerMove.isWallTouched
            && player.PlayerMove.keyboardLeft == player.PlayerMove.lastWallIsLeft) player.ChangeState<WallHoldState>();
    }

    public void Exit(PlayerController player) { }
}
