using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JumpState : AirSubState
{
    private float minWallHoldTime = 1f; // 이 초가 지나야 벽 짚기가 가능함
    private float elapsedTime;
    
    public override void Enter(PlayerController controller)
    {
        base.Enter(controller);
        controller.Condition.canStaminaRecovery.Value = true;
        controller.Animator.SetTriggerAnimation(PlayerAnimID.Jump);
        controller.isLookLocked = true; 
        elapsedTime = 0f;
        
        if (controller.Move.isWallTouched)
        {
            controller.Move.PlaceJump();
        }
        else
        {
            controller.Move.Jump();
        }
        
        if (!controller.Move.isGroundJump) controller.Move.isGroundJump = true;
    }

    public override void HandleInput(PlayerController player)
    {
        elapsedTime += Time.deltaTime;
        var moveInput = player.Inputs.Player.Move.ReadValue<Vector2>();
        
        if (player.Inputs.Player.Jump.triggered)
        {
            if(!player.Move.isDoubleJump)
            {
                player.ChangeState<DoubleJumpState>();
                return;
            }
        }
        if (player.Move.isWallTouched && elapsedTime >= minWallHoldTime)
        {
            player.ChangeState<WallHoldState>();
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
        if (!player.Move.isGroundJump)
        {
            player.Move.isGroundJump = true;
            return;
        }
        player.Move.Move();
        if (player.Move.rb.velocity.y < 0)
        {
            player.ChangeState<FallState>();
            return;
        }
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
        player.isLookLocked = false;
    }
}
