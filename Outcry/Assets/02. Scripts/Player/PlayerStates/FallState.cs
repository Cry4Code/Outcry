using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.isLookLocked = true; 
        player.PlayerAnimator.SetBoolAnimation(PlayerAnimID.Fall);
        player.PlayerMove.rb.gravityScale = 2.5f;
    }

    public void Exit(PlayerController player)
    {
        player.PlayerMove.rb.gravityScale = 1f;
        player.PlayerAttack.HasJumpAttack = false;
        player.isLookLocked = false; 
    }

    public void HandleInput(PlayerController player)
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
        
        
    }

    public void LogicUpdate(PlayerController player)
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
