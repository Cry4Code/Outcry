using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoubleJumpState : AirSubState
{
    private float elapsedTime;
    private float fallStartTime = 0.1f;
    
    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        Debug.Log("[플레이어] !!Double Jump!!");
        player.SetAnimation(PlayerAnimID.DoubleJump, true);
        player.PlayerMove.DoubleJump();
        
    }
    
    public override void HandleInput(PlayerController player) 
    {
        elapsedTime += Time.deltaTime;
        var moveInput = player.Inputs.Player.Move.ReadValue<Vector2>();
        
        if(player.PlayerMove.isWallTouched 
           && ((moveInput.x < 0 && player.PlayerMove.lastWallIsLeft) || (moveInput.x > 0 && !player.PlayerMove.lastWallIsLeft)))
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
            player.PlayerMove.Move();

        if (elapsedTime > fallStartTime)
        {
            if (player.PlayerMove.rb.velocity.y < 0)
            {
                player.ChangeState<FallState>();
                return;
            }
        }
        if (player.PlayerMove.isGrounded)
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
        
        if (player.Inputs.Player.NormalAttack.triggered && !player.PlayerAttack.HasJumpAttack)
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
