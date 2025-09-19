using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DodgeState : IPlayerState
{
    
    private float startStateTime;
    private float startAttackTime = 0.01f;
    private float animRunningTime = 0f;
    private float dodgePower = 20f;
    private float dodgeAnimationLength;
    private Vector2 dodgeDirection;
    
    public void Enter(PlayerController controller)
    {
        if (!controller.Condition.TryUseStamina(controller.Data.dodgeStamina))
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
        
        var moveInputs = controller.Inputs.Player.Move.ReadValue<Vector2>();
        controller.isLookLocked = false;
        controller.Move.rb.velocity = Vector2.zero;
        if (moveInputs.x != 0)
        {
            // 입력이 있을 때 그 쪽 보게
            dodgeDirection = (moveInputs.x < 0 ? Vector2.left : Vector2.right) * dodgePower;
            controller.Move.ForceLook(moveInputs.x < 0);
        }
        else
        {
            // 입력이 없으면 그냥 보던 쪽으로 가게
            dodgeDirection = (controller.transform.localScale.x < 0 ? Vector2.left : Vector2.right) * dodgePower;
            controller.Move.ForceLook(controller.transform.localScale.x < 0);
        }
        controller.Move.isDodged = true;
        controller.Animator.ClearTrigger();
        controller.Animator.ClearInt();
        controller.Animator.ClearBool();
        controller.Inputs.Player.Move.Disable();
        dodgeAnimationLength = 
            controller.Animator.animator.runtimeAnimatorController
                .animationClips.First(c => c.name == "Dodge").length;
        controller.Move.rb.AddForce(dodgeDirection, ForceMode2D.Impulse);
        controller.Animator.SetTriggerAnimation(PlayerAnimID.Dodge);
        controller.Condition.SetInvincible(controller.Data.dodgeInvincibleTime);
        controller.isLookLocked = true;
        startStateTime = Time.time;
        animRunningTime = 0f;
    }

    public void HandleInput(PlayerController player)
    {
        
    }

    public void LogicUpdate(PlayerController player)
    {
        animRunningTime += Time.deltaTime;
        
        if (Time.time - startStateTime > startAttackTime)
        {
            AnimatorStateInfo curAnimInfo = player.Animator.animator.GetCurrentAnimatorStateInfo(0);

            if (curAnimInfo.IsName("Dodge"))
            { 
                float animTime = curAnimInfo.normalizedTime;

                if (animTime >= 1.0f)
                {
                    if (player.Move.isGrounded) player.ChangeState<IdleState>();
                    else player.ChangeState<FallState>();
                    return;
                }
            }

            if (animRunningTime >= dodgeAnimationLength)
            {
                if (player.Move.isGrounded) player.ChangeState<IdleState>();
                else player.ChangeState<FallState>();
                return;
            }
                
        }
    }

    public void Exit(PlayerController player)
    {
        player.Inputs.Player.Move.Enable();
    }
}
