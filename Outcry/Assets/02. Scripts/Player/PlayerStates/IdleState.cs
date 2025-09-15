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
        player.PlayerAttack.ClearAttackCount();
        player.PlayerAnimator.ClearTrigger();
        player.PlayerAnimator.ClearInt();
        player.isLookLocked = false;
        
        player.PlayerAnimator.SetBoolAnimation(PlayerAnimID.Idle);
        player.Inputs.Player.Move.Enable();
    }

    public void HandleInput(PlayerController player)
    {
        var input = player.Inputs.Player.Move.ReadValue<Vector2>();
        if (player.Inputs.Player.Jump.triggered && player.PlayerMove.isGrounded && !player.PlayerMove.isGroundJump)
        {
            // Debug.Log("Jump Key Input");
            player.ChangeState<JumpState>();
            return;
        }
        if (input.x != 0)
        {
            player.PlayerMove.ForceLook(input.x < 0);
            player.isLookLocked = true;
            player.ChangeState<MoveState>();
            return;
        }

        if (player.Inputs.Player.NormalAttack.triggered)
        {
            player.isLookLocked = true;
            player.ChangeState<NormalAttackState>();
            return;
        }
    }

    public void LogicUpdate(PlayerController player) 
    {
        if (player.PlayerMove.rb.velocity.y < 0) player.ChangeState<FallState>();
    }
    public void Exit(PlayerController player) { }
}