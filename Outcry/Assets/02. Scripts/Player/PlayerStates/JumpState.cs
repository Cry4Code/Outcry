using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : AirSubState
{
    private float minWallHoldTime = 1f; // 이 초가 지나야 벽 짚기가 가능함
    private float elapsedTime;
    
    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        player.PlayerAnimator.SetTriggerAnimation(PlayerAnimID.Jump);
        player.isLookLocked = true; 
        elapsedTime = 0f;
        if (player.PlayerMove.isWallTouched)
        {
            player.PlayerMove.PlaceJump();
        }
        else
        {
            player.PlayerMove.Jump();
        }
        if (!player.PlayerMove.isGroundJump) player.PlayerMove.isGroundJump = true;
    }

    public override void HandleInput(PlayerController player)
    {
        elapsedTime += Time.deltaTime;
        var moveInput = player.Inputs.Player.Move.ReadValue<Vector2>();
        
        if (player.Inputs.Player.Jump.triggered)
        {
            if(!player.PlayerMove.isDoubleJump)
            {
                player.ChangeState<DoubleJumpState>();
                return;
            }
        }
        if (player.PlayerMove.isWallTouched && elapsedTime >= minWallHoldTime)
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
        if (player.Inputs.Player.NormalAttack.triggered && !player.PlayerAttack.HasJumpAttack)
        {
            player.isLookLocked = true;
            player.ChangeState<NormalJumpAttackState>();
            return;
        }
    }

    public override void LogicUpdate(PlayerController player)
    {
        if (!player.PlayerMove.isGroundJump)
        {
            player.PlayerMove.isGroundJump = true;
            return;
        }
        player.PlayerMove.Move();
        if (player.PlayerMove.rb.velocity.y < 0)
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
