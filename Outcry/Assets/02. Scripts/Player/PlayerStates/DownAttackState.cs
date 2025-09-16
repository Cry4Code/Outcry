using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownAttackState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.PlayerAnimator.ClearBool();
        player.isLookLocked = true; 
        player.PlayerAnimator.SetTriggerAnimation(PlayerAnimID.DownAttack);
        player.Inputs.Player.Move.Disable();
        player.PlayerMove.rb.gravityScale = 3.5f;
    }

    public void HandleInput(PlayerController player)
    {
        
    }

    public void LogicUpdate(PlayerController player)
    {
        if (player.PlayerMove.isGrounded)
        {
            player.ChangeState<IdleState>();
            return;
        }
            
    }

    public void Exit(PlayerController player)
    {
        player.PlayerMove.rb.gravityScale = 1f;
    }
}
