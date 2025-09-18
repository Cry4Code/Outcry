using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedState : IPlayerState
{
    
    private float startStateTime;
    private float startAttackTime = 0.01f;
    private float canInputTime = 0.1f;
    
    public void Enter(PlayerController player)
    {
        player.PlayerMove.rb.velocity = Vector2.zero;
        player.PlayerAnimator.SetTriggerAnimation(PlayerAnimID.Damaged);
        player.Inputs.Disable();
    }

    public void HandleInput(PlayerController player)
    {
        if (Time.time - startStateTime > canInputTime)
        {
            player.Inputs.Enable();
        }
    }

    public void LogicUpdate(PlayerController player)
    {
        if (Time.time - startStateTime > startAttackTime)
        {
            AnimatorStateInfo curAnimInfo = player.PlayerAnimator.animator.GetCurrentAnimatorStateInfo(0);

            if (curAnimInfo.IsName("Damaged"))
            { 
                float animTime = curAnimInfo.normalizedTime;

                if (animTime >= 1.0f)
                {
                    player.ChangeState<IdleState>();
                    return;
                }
            }
        }
    }

    public void Exit(PlayerController player)
    {
        
    }
}
