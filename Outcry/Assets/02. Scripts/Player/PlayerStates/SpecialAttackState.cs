using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialAttackState : IPlayerState
{
    private float startStateTime;
    private float startAttackTime = 0.01f;
    private float animRunningTime = 0f;
    private float attackAnimationLength;
    private float specialAttackPower = 20f;
    private Vector2 specialAttackDirection;
    private float cursorAngle = 0f;
    
    public void Enter(PlayerController player)
    {
        player.isLookLocked = false;
        player.PlayerMove.ForceLook(CursorManager.Instance.mousePosition.x - player.transform.position.x < 0);
        player.PlayerMove.rb.velocity = Vector2.zero;
        player.PlayerAnimator.ClearTrigger();
        player.PlayerAnimator.ClearInt();
        player.PlayerAnimator.ClearBool();
        player.Inputs.Player.Move.Disable();
        animRunningTime = 0f;
        attackAnimationLength = 
            player.PlayerAnimator.animator.runtimeAnimatorController
                .animationClips.First(c => c.name == "SpecialAttack").length;
        specialAttackDirection = (CursorManager.Instance.mousePosition - player.transform.position).normalized 
            * specialAttackPower;
        player.PlayerAnimator.SetTriggerAnimation(PlayerAnimID.SpecialAttack);
        player.PlayerMove.rb.AddForce(specialAttackDirection, ForceMode2D.Impulse);
        player.isLookLocked = true;
        
        // 마우스 바라보는 방향으로 캐릭터 돌리기
        // 1. 마우스를 바라보는 각도 구하기
        cursorAngle = Mathf.Atan2(specialAttackDirection.y, specialAttackDirection.x) *  Mathf.Rad2Deg;
        
        // 2. 그 각도대로 돌리기
        if (specialAttackDirection.x > 0)
        {
            player.transform.rotation = Quaternion.Euler(0, 0, cursorAngle);
        }
        else
        {
            player.transform.rotation = Quaternion.Euler(0, 0, -180f+cursorAngle);
        }   


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

            if (curAnimInfo.IsName("SpecialAttack"))
            { 
                float animTime = curAnimInfo.normalizedTime;

                if (animTime >= 1.0f)
                {
                    if (player.PlayerMove.isGrounded) player.ChangeState<IdleState>();
                    else player.ChangeState<FallState>();
                    return;
                }
            }

            if (animRunningTime >= attackAnimationLength)
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
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
