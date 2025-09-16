using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : GroundSubState
{
    public override void Enter(PlayerController player)
    {
        // 애니메이션 설정 
        // player.SetAnimation("Idle");
        base.Enter(player);
        player.PlayerMove.Stop();
        player.PlayerMove.ChangeGravity(false);
        player.PlayerAttack.ClearAttackCount();
        player.PlayerAnimator.ClearTrigger();
        player.PlayerAnimator.ClearInt();
        player.PlayerAnimator.ClearBool();
        
        player.PlayerAnimator.OnBoolParam(PlayerAnimID.Idle);
        player.Inputs.Player.Move.Enable();
    }

    public override void HandleInput(PlayerController player)
    {
        AnimatorStateInfo curAnimInfo = player.PlayerAnimator.animator.GetCurrentAnimatorStateInfo(0);
        if (curAnimInfo.IsName("Idle"))
        {
            player.isLookLocked = false;
        }
        else
        {
            return;
        }
        
        var input = player.Inputs.Player.Move.ReadValue<Vector2>();
        
        if (player.Inputs.Player.NormalAttack.triggered)
        {
            player.isLookLocked = true;
            player.ChangeState<NormalAttackState>();
            return;
        }
        
        if (player.Inputs.Player.Jump.triggered 
            && player.PlayerMove.isGrounded 
            && !player.PlayerMove.isGroundJump 
            && !player.PlayerMove.isWallTouched)
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

        
    }
 
    public override void LogicUpdate(PlayerController player) 
    {
        
        
        if (player.PlayerMove.rb.velocity.y < 0) player.ChangeState<FallState>();
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
        player.PlayerAnimator.OffBoolParam(PlayerAnimID.Idle);
    }
    
}