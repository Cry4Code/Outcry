using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StartParryState : IPlayerState
{
    private float startStateTime;
    private float startAttackTime = 0.01f;
    public void Enter(PlayerController controller)
    {
        if (!controller.Condition.TryUseStamina(controller.Data.parryStamina))
        {
            if (controller.Move.isGrounded)
            {
                controller.ChangeState<IdleState>();
                return;
            }
            else
            {
                controller.ChangeState<FallState>();
                return;
            }
        }
        controller.isLookLocked = false;
        controller.Move.ForceLook(CursorManager.Instance.mousePosition.x - controller.transform.position.x < 0);
        controller.Move.rb.velocity = Vector2.zero;
        controller.Animator.ClearTrigger();
        controller.Animator.ClearInt();
        controller.Animator.ClearBool();
        controller.Inputs.Player.Move.Disable();
        controller.Animator.SetTriggerAnimation(PlayerAnimID.StartParry);
        
        controller.isLookLocked = true;
        controller.Attack.isStartParry = true;
    }

    public void HandleInput(PlayerController player)
    {
        
    }

    public void LogicUpdate(PlayerController player)
    {
        if (player.Attack.successParry)
        {
            player.ChangeState<SuccessParryState>();
            return;
        }
        
        if (Time.time - startStateTime > startAttackTime)
        {
            AnimatorStateInfo curAnimInfo = player.Animator.animator.GetCurrentAnimatorStateInfo(0);

            if (curAnimInfo.IsName("StartParry"))
            { 
                float animTime = curAnimInfo.normalizedTime;

                if (animTime >= 1.0f)
                {
                    if (player.Move.isGrounded) player.ChangeState<IdleState>();
                    else player.ChangeState<FallState>();
                    return;
                }
            }
        }
    }

    public void Exit(PlayerController player)
    {
        player.Attack.isStartParry = false;
    }
}
