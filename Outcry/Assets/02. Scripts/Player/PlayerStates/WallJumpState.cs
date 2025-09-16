using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpState : AirSubState
{
    private float wallJumpStartTime;
    private float wallHoldAbleTime = 0.5f;

    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        // Debug.Log("벽점!");
        // 벽점할 때에는 벽 반대방향 봐야됨
        player.PlayerMove.ForceLook(!player.PlayerMove.lastWallIsLeft);
        player.isLookLocked = true;
        // 벽점했으니까 강제로 벽 터치 취소
        player.PlayerAnimator.ClearBool(); // WallHold 끄려고
        player.PlayerMove.isWallTouched = false;
        player.PlayerAnimator.SetTriggerAnimation(PlayerAnimID.WallJump);
        
        wallJumpStartTime = Time.time;
        player.PlayerMove.WallJump();
    }

    public override void HandleInput(PlayerController player) 
    {
        //if (player.Inputs.Player.Move.ReadValue<Vector2>().x != 0)
        //{
        //    player.PlayerMove.Move();
        //    return;
        //}

        var moveInputs = player.Inputs.Player.Move.ReadValue<Vector2>();

        if (player.Inputs.Player.Jump.triggered && !player.PlayerMove.isDoubleJump)
        {
            player.ChangeState<DoubleJumpState>();
            return;
        }

        if(Time.time - wallJumpStartTime > wallHoldAbleTime && player.PlayerMove.isWallTouched)
        {
            player.ChangeState<WallHoldState>();
            return;
        }
        else
        {
            player.PlayerMove.isWallTouched = false;
        }
        
        if (player.Inputs.Player.NormalAttack.triggered && moveInputs.y < 0)
        {
            player.isLookLocked = true;
            player.ChangeState<DownAttackState>();
            return;
        }
        
        if (player.Inputs.Player.NormalAttack.triggered && !player.PlayerAttack.HasJumpAttack)
        {
            player.isLookLocked = true;
            player.ChangeState<NormalJumpAttackState>();
            return;
        }
        
        

    }

    public override void LogicUpdate(PlayerController player)
    {
        
        if (player.PlayerMove.rb.velocity.y < 0)
        {
            player.ChangeState<FallState>();
            return;
        }
        if (player.PlayerMove.isGrounded)
        {
            player.ChangeState<IdleState>();
            return;
        }
    }

    public override void Exit(PlayerController player) 
    {
        base.Exit(player);
        player.isLookLocked = false;
    }
}
