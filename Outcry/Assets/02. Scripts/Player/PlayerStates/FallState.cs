using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : AirSubState
{
    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        player.isLookLocked = true; 
        player.PlayerAnimator.SetBoolAnimation(PlayerAnimID.Fall);
        player.PlayerMove.rb.gravityScale = 2.5f;
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
        player.PlayerMove.rb.gravityScale = 1f;
        player.PlayerAttack.HasJumpAttack = false;
        player.isLookLocked = false; 
    }

    public override void HandleInput(PlayerController player)
    {
        var moveInputs = player.Inputs.Player.Move.ReadValue<Vector2>();

        if (player.PlayerMove.isWallTouched
            && ((player.PlayerMove.lastWallIsLeft && moveInputs.x < 0) || (!player.PlayerMove.lastWallIsLeft && moveInputs.x > 0)))
        {

            player.ChangeState<WallHoldState>();
            return;
        }


        if (player.Inputs.Player.Jump.triggered)
        {
            /*if(!player.PlayerMove.isGroundJump)
            {
                player.ChangeState<JumpState>();
                return;
            }*/
            if (!player.PlayerMove.isDoubleJump)
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
        
        
        if (player.Inputs.Player.NormalAttack.triggered && !player.PlayerAttack.HasJumpAttack)
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
                player.PlayerMove.ForceLook(input.x < 0);
            }
            player.PlayerMove.Move();
        }

        if (player.PlayerMove.isGrounded)
        {
            player.ChangeState<IdleState>();
            return;
        }
        

        
    }
}
