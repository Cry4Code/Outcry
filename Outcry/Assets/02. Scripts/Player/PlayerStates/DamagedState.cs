using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedState : IPlayerState
{
    
    private float startStateTime;
    private float startAttackTime = 0.01f;
    private float canInputTime = 0.1f;
    
    public void Enter(PlayerController controller)
    {
        controller.Move.rb.velocity = Vector2.zero;
        controller.Condition.canStaminaRecovery.Value = true;
        controller.Animator.SetTriggerAnimation(PlayerAnimID.Damaged);
        controller.Inputs.Disable();
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
            AnimatorStateInfo curAnimInfo = player.Animator.animator.GetCurrentAnimatorStateInfo(0);

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
