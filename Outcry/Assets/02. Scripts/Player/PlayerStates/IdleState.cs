using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        // 애니메이션 설정 
        // player.SetAnimation("Idle");

        player.PlayerMove.Stop();
        player.PlayerMove.ChangeGravity(false);
        player.PlayerAnimator.ClearTrigger();
        player.PlayerAnimator.SetBoolAnimation(PlayerAnimID.Idle);
    }

    public void HandleInput(PlayerController player)
    {
        var input = player.Inputs.Player.Move.ReadValue<Vector2>();
        if (player.Inputs.Player.Jump.triggered && player.PlayerMove.isGrounded && !player.PlayerMove.isGroundJump)
        {
            // Debug.Log("Jump Key Input");
            player.ChangeState<JumpState>();
        }
        else if (input.x != 0)
        {
            player.PlayerMove.ForceLook(input.x < 0);
            player.isLookLocked = true;
            player.ChangeState<MoveState>();

        }// 이동이 있으면 MoveState로 이동
        // else if (player.Inputs.Player.Dodge.triggered) player.ChangeState(new DodgeState());
    }

    public void LogicUpdate(PlayerController player) 
    {
        if (player.PlayerMove.rb.velocity.y < 0) player.ChangeState<FallState>();
    }
    public void Exit(PlayerController player) { }
}