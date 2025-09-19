using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoubleJumpState : AirSubState
{
    private float elapsedTime;
    private float fallStartTime = 0.1f;
    
    public override void Enter(PlayerController controller)
    {
        if (!controller.Condition.TryUseStamina(controller.Data.doubleJumpStamina))
        {
            if (controller.Move.isGrounded)
            {
                controller.ChangeState<IdleState>();
                return;
            }
            else
            {
                controller.ChangeState<FallState>();
                return;
            }
        }
        base.Enter(controller);
        Debug.Log("[플레이어] !!Double Jump!!");
        controller.SetAnimation(PlayerAnimID.DoubleJump, true);
        controller.Move.DoubleJump();
        
    }
    
    public override void HandleInput(PlayerController player) 
    {
        elapsedTime += Time.deltaTime;
        var moveInput = player.Inputs.Player.Move.ReadValue<Vector2>();
        
        if(player.Move.isWallTouched 
           && ((moveInput.x < 0 && player.Move.lastWallIsLeft) || (moveInput.x > 0 && !player.Move.lastWallIsLeft)))
        {
            player.ChangeState<WallHoldState>();
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
        if (player.Inputs.Player.Parry.triggered)
        {
            player.ChangeState<StartParryState>();
            return;
        }
    }

    public override void LogicUpdate(PlayerController player)
    {
        var moveInput = player.Inputs.Player.Move.ReadValue<Vector2>();
        if(moveInput != null)
            player.Move.Move();

        if (elapsedTime > fallStartTime)
        {
            if (player.Move.rb.velocity.y < 0)
            {
                player.ChangeState<FallState>();
                return;
            }
        }
        if (player.Move.isGrounded)
        {
            player.ChangeState<IdleState>();
            return;
        }
        
        if (player.Inputs.Player.NormalAttack.triggered && moveInput.y < 0)
        {
            player.isLookLocked = true;
            player.ChangeState<DownAttackState>();
            return;
        }
        
        if (player.Inputs.Player.NormalAttack.triggered && !player.Attack.HasJumpAttack)
        {
            player.isLookLocked = true;
            player.ChangeState<NormalJumpAttackState>();
            return;
        }
        
        
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
    }
}
