using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHoldState : AirSubState
{
    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        player.PlayerMove.ForceLook(!player.PlayerMove.lastWallIsLeft);
        player.PlayerAttack.ClearAttackCount();
        player.isLookLocked = true;
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
        player.isLookLocked = false;
        // player.PlayerAnimator.ClearBool();
    }

    public override void HandleInput(PlayerController player)
    {
        var moveInput = player.Inputs.Player.Move.ReadValue<Vector2>();
        if (!player.PlayerMove.isWallTouched)
        {
            player.ChangeState<FallState>();
            return;
        }

        // 벽이 있는 방향으로 입력이 들어왔을 때
        if (((moveInput.x < 0 && player.PlayerMove.lastWallIsLeft) 
            || moveInput.x > 0 && !player.PlayerMove.lastWallIsLeft) )
        {
            // 점프 키가 눌림 and 벽점 가능함
            if(player.Inputs.Player.Jump.triggered && player.PlayerMove.CanWallJump())
            {
                // Debug.Log("벽점으로");
                player.ChangeState<WallJumpState>();
                return;
            }
            if (player.PlayerMove.isWallTouched)
            {
                // Debug.Log("중력 감소");
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
        
        if (player.Inputs.Player.SpecialAttack.triggered)
        {
            player.isLookLocked = false;
            player.ChangeState<SpecialAttackState>();
            return;
        }
        if (player.Inputs.Player.Dodge.triggered)
        {
            player.ChangeState<DodgeState>();
            return;
        }
        
    }

    public override void LogicUpdate(PlayerController player)
    {
        if (player.PlayerMove.rb.velocity.y < 0)
        {
            player.PlayerAnimator.SetBoolAnimation(PlayerAnimID.WallHold);
        }
        
        if (player.PlayerMove.keyboardLeft != player.PlayerMove.lastWallIsLeft)
        {
            player.PlayerMove.Move();
            return;
        }
    }
}
