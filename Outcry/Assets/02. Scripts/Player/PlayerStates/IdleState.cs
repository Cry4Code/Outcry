using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : GroundSubState
{
    public override void Enter(PlayerController controller)
    {
        // 애니메이션 설정 
        // player.SetAnimation("Idle");
        base.Enter(controller);
        
        controller.Move.Stop();
        controller.Move.ChangeGravity(false);
        controller.Condition.canStaminaRecovery.Value = true;
        controller.Attack.ClearAttackCount();
        controller.Animator.ClearTrigger();
        controller.Animator.ClearInt();
        controller.Animator.ClearBool();
        
        controller.Animator.OnBoolParam(PlayerAnimID.Idle);
        controller.Inputs.Player.Move.Enable();
    }

    public override void HandleInput(PlayerController player)
    {
        AnimatorStateInfo curAnimInfo = player.Animator.animator.GetCurrentAnimatorStateInfo(0);
        if (curAnimInfo.IsName("Idle"))
        {
            player.isLookLocked = false;
        }
        else
        {
            player.Move.Stop();
            return;
        }
        
        var input = player.Inputs.Player.Move.ReadValue<Vector2>();
        
        if (player.Inputs.Player.NormalAttack.triggered)
        {
            player.isLookLocked = true;
            player.ChangeState<NormalAttackState>();
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
        
        
        if (player.Inputs.Player.Jump.triggered 
            && player.Move.isGrounded 
            && !player.Move.isGroundJump 
            && !player.Move.isWallTouched)
        {
            // Debug.Log("Jump Key Input");
            player.ChangeState<JumpState>();
            return;
        }
        if (input.x != 0)
        {
            player.Move.ForceLook(input.x < 0);
            player.isLookLocked = true;
            player.ChangeState<MoveState>();
            return;
        }

        
    }
 
    public override void LogicUpdate(PlayerController player) 
    {
        

        if (player.Move.isDodged)
        {
            player.Move.Stop();
            player.Move.isDodged = false;
        }
        
        if (player.Move.rb.velocity.y < 0) player.ChangeState<FallState>();
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
        player.Animator.OffBoolParam(PlayerAnimID.Idle);
    }
    
}