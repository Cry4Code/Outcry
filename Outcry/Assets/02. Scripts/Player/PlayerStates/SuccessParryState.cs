using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccessParryState : IPlayerState
{
    private float startStateTime;
    private float startAttackTime = 0.01f;
    public void Enter(PlayerController controller)
    {
        controller.isLookLocked = false;
        controller.Move.ForceLook(CursorManager.Instance.mousePosition.x - controller.transform.position.x < 0);
        controller.Move.rb.velocity = Vector2.zero;
        controller.Animator.ClearTrigger();
        controller.Animator.ClearInt();
        controller.Animator.ClearBool();
        controller.Inputs.Player.Move.Disable();
        controller.Hitbox.Damage = controller.Data.parryDamage;
        controller.Animator.SetTriggerAnimation(PlayerAnimID.SuccessParry);
        
        controller.isLookLocked = true;
    }

    public void HandleInput(PlayerController player)
    {
        
    }

    public void LogicUpdate(PlayerController player)
    {
        if (Time.time - startStateTime > startAttackTime)
        {
            AnimatorStateInfo curAnimInfo = player.Animator.animator.GetCurrentAnimatorStateInfo(0);

            if (curAnimInfo.IsName("SuccessParry"))
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
        player.Attack.successParry = false;
    }
}
