using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : AirSubState
{
    public override void Enter(PlayerController controller)
    {
        base.Enter(controller);
        controller.isLookLocked = true; 
        controller.Condition.canStaminaRecovery.Value = true;
        controller.Animator.SetBoolAnimation(PlayerAnimID.Fall);
        controller.Move.rb.gravityScale = 6f;
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
        player.Move.rb.gravityScale = 1f;
        player.Attack.HasJumpAttack = false;
        player.isLookLocked = false; 
    }

    public override void HandleInput(PlayerController player)
    {
        var moveInputs = player.Inputs.Player.Move.ReadValue<Vector2>();

        if (player.Move.isWallTouched
            && ((player.Move.lastWallIsLeft && moveInputs.x < 0) || (!player.Move.lastWallIsLeft && moveInputs.x > 0)))
        {

            player.ChangeState<WallHoldState>();
            return;
        }


        if (player.Inputs.Player.Jump.triggered)
        {
            if (!player.Move.isDoubleJump)
            {
                player.ChangeState<DoubleJumpState>();
                return;
            }
        }
        
        if (player.Inputs.Player.NormalAttack.triggered && moveInputs.y < 0)
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

        // 공중에서 이동 가능
        var input = player.Inputs.Player.Move.ReadValue<Vector2>();
        if (input != null)
        {
            if (input.x != 0)
            {
                player.Move.ForceLook(input.x < 0);
            }
            player.Move.Move();
        }

        if (player.Move.isGrounded)
        {
            player.ChangeState<IdleState>();
            return;
        }
        

        
    }
}
