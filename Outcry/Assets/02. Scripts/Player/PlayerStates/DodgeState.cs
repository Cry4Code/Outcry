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
    private float dodgeInvincibleTime = 0.3f;
    
    private int needStamina = 15;
    
    public void Enter(PlayerController player)
    {
        if (!player.Condition.TryUseStamina(needStamina))
        {
            if (player.PlayerMove.isGrounded)
            {
                player.ChangeState<IdleState>();
                return;
            }
            else
            {
                player.ChangeState<FallState>();
                return;
            }
        }
        
        var moveInputs = player.Inputs.Player.Move.ReadValue<Vector2>();
        player.isLookLocked = false;
        player.PlayerMove.rb.velocity = Vector2.zero;
        if (moveInputs.x != 0)
        {
            // 입력이 있을 때 그 쪽 보게
            dodgeDirection = (moveInputs.x < 0 ? Vector2.left : Vector2.right) * dodgePower;
            player.PlayerMove.ForceLook(moveInputs.x < 0);
        }
        else
        {
            // 입력이 없으면 그냥 보던 쪽으로 가게
            dodgeDirection = (player.transform.localScale.x < 0 ? Vector2.left : Vector2.right) * dodgePower;
            player.PlayerMove.ForceLook(player.transform.localScale.x < 0);
        }
        player.PlayerMove.isDodged = true;
        player.PlayerAnimator.ClearTrigger();
        player.PlayerAnimator.ClearInt();
        player.PlayerAnimator.ClearBool();
        player.Inputs.Player.Move.Disable();
        dodgeAnimationLength = 
            player.PlayerAnimator.animator.runtimeAnimatorController
                .animationClips.First(c => c.name == "Dodge").length;
        player.PlayerMove.rb.AddForce(dodgeDirection, ForceMode2D.Impulse);
        player.PlayerAnimator.SetTriggerAnimation(PlayerAnimID.Dodge);
        player.Condition.SetInvincible(dodgeInvincibleTime);
        player.isLookLocked = true;
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
            AnimatorStateInfo curAnimInfo = player.PlayerAnimator.animator.GetCurrentAnimatorStateInfo(0);

            if (curAnimInfo.IsName("Dodge"))
            { 
                float animTime = curAnimInfo.normalizedTime;

                if (animTime >= 1.0f)
                {
                    if (player.PlayerMove.isGrounded) player.ChangeState<IdleState>();
                    else player.ChangeState<FallState>();
                    return;
                }
            }

            if (animRunningTime >= dodgeAnimationLength)
            {
                if (player.PlayerMove.isGrounded) player.ChangeState<IdleState>();
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
