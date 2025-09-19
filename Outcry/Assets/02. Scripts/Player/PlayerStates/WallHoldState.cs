using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHoldState : AirSubState
{
    public override void Enter(PlayerController controller)
    {
        base.Enter(controller);
        controller.Move.ForceLook(!controller.Move.lastWallIsLeft);
        controller.Attack.ClearAttackCount();
        controller.isLookLocked = true;
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
        player.isLookLocked = false;
        player.Condition.canStaminaRecovery.Value = true;
        // player.PlayerAnimator.ClearBool();
    }

    public override void HandleInput(PlayerController player)
    {
        var moveInput = player.Inputs.Player.Move.ReadValue<Vector2>();
        if (!player.Move.isWallTouched)
        {
            player.ChangeState<FallState>();
            return;
        }

        // 벽이 있는 방향으로 입력이 들어왔을 때
        if (((moveInput.x < 0 && player.Move.lastWallIsLeft) 
            || moveInput.x > 0 && !player.Move.lastWallIsLeft) )
        {
            // 점프 키가 눌림 and 벽점 가능함
            if(player.Inputs.Player.Jump.triggered && player.Move.CanWallJump())
            {
                // Debug.Log("벽점으로");
                player.ChangeState<WallJumpState>();
                return;
            }
            if (player.Move.isWallTouched)
            {
                // Debug.Log("중력 감소");
                player.Move.ChangeGravity(true);
                return;
            }
        }

        if (moveInput.x == 0)
        {
            player.ChangeState<FallState>();
            return;
        }
        if (player.Move.isGrounded)
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
        if (player.Move.rb.velocity.y < 0)
        {
            player.Animator.SetBoolAnimation(PlayerAnimID.WallHold);
        }
        
        if (player.Move.keyboardLeft != player.Move.lastWallIsLeft)
        {
            player.Move.Move();
            return;
        }
    }
}
