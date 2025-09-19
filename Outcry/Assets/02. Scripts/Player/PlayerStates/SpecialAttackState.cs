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
    private float specialAttackSpeed = 10f;
    private Vector2 specialAttackDirection;
    private float specialAttackDistance = 7f;
    private Vector2 startPos;
    private Vector2 targetPos;
    private Vector2 newPos;
    private Vector2 curPos;
    private float cursorAngle = 0f;


    private float t;
    public void Enter(PlayerController controller)
    {
        if (!controller.Condition.TryUseStamina(controller.Data.specialAttackStamina))
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
        animRunningTime = 0f;
        attackAnimationLength = 
            controller.Animator.animator.runtimeAnimatorController
                .animationClips.First(c => c.name == "SpecialAttack").length;
        specialAttackDirection = (CursorManager.Instance.mousePosition - controller.transform.position).normalized;
        
        controller.Animator.SetTriggerAnimation(PlayerAnimID.SpecialAttack);
        
        controller.isLookLocked = true;
        
        // 마우스 바라보는 방향으로 캐릭터 돌리기
        // 1. 마우스를 바라보는 각도 구하기
        cursorAngle = Mathf.Atan2(specialAttackDirection.y, specialAttackDirection.x) *  Mathf.Rad2Deg;
        
        // 2. 그 각도대로 돌리기
        if (specialAttackDirection.x > 0)
        {
            controller.transform.rotation = Quaternion.Euler(0, 0, cursorAngle);
        }
        else
        {
            controller.transform.rotation = Quaternion.Euler(0, 0, -180f+cursorAngle);
        }
        
        startPos = controller.transform.position;
        targetPos = startPos + (specialAttackDirection * specialAttackDistance);
    }

    public void HandleInput(PlayerController player)
    {
        
    }

    public void LogicUpdate(PlayerController player)
    {
        animRunningTime += Time.deltaTime;
        t = animRunningTime / attackAnimationLength;

        newPos = Vector2.MoveTowards(startPos, targetPos, t * specialAttackSpeed);

        curPos = player.transform.position;
        
        
        // 현재 위치에서 이동할 위치만큼 선 하나 그어서, 그게 벽에 닿으면 벽 끝에까지만 가고 상태 바뀌게함
        Vector2 direction = (newPos - curPos).normalized;
        float distance = Vector2.Distance(curPos, newPos);
        
        RaycastHit2D hit =
            Physics2D.Raycast(player.transform.position, direction, distance, player.Move.groundMask);
        
        if (hit.collider != null)
        {
             player.Move.rb.MovePosition(hit.point - direction * 0.01f);
            if (player.Move.isGrounded) player.ChangeState<IdleState>();
            else player.ChangeState<FallState>();
            return;
        }
        
        
        player.Move.rb.MovePosition(newPos);
        
        if (Vector2.Distance(newPos, targetPos) < 0.01f)
        {
            player.Move.rb.velocity = Vector2.zero;
            if (player.Move.isGrounded) player.ChangeState<IdleState>();
            else player.ChangeState<FallState>();
            return;
        }
        
        if (Time.time - startStateTime > startAttackTime)
        {
            AnimatorStateInfo curAnimInfo = player.Animator.animator.GetCurrentAnimatorStateInfo(0);

            if (curAnimInfo.IsName("SpecialAttack"))
            { 
                float animTime = curAnimInfo.normalizedTime;

                if (animTime >= 1.0f)
                {
                    if (player.Move.isGrounded) player.ChangeState<IdleState>();
                    else player.ChangeState<FallState>();
                    return;
                }
            }

            if (animRunningTime >= attackAnimationLength)
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
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
