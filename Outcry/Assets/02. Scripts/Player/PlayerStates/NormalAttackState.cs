using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalAttackState : NormalAttackSubState
{
    private float startStateTime;
    private float startAttackTime = 0.01f;
    private float comboTime = 0.5f; // 콤보타임 지나서 누르면 의미없음.
    private bool isComboInput = false;
    private float attackAnimationLength;
    private float animRunningTime = 0f;
    
    public override void Enter(PlayerController controller)
    {
        base.Enter(controller);
        startStateTime = Time.time;
        isComboInput = false;
        controller.Condition.canStaminaRecovery.Value = false;
        // AttackCount = 0 + NormalAttack Trigger On.
        controller.Animator.ClearBool();
        controller.Hitbox.Damage = controller.Data.normalAttackDamage[controller.Attack.AttackCount];
        controller.Animator.SetTriggerAnimation(PlayerAnimID.NormalAttack);
        controller.Inputs.Player.Move.Disable();
        animRunningTime = 0f;
        attackAnimationLength = 
            controller.Animator.animator.runtimeAnimatorController
                .animationClips.First(c => c.name == $"NormalAttack_{controller.Attack.AttackCount}").length;
        
    }

    public override void HandleInput(PlayerController player)
    {
        player.Move.rb.velocity = Vector2.zero;
        // 키 입력이 필요
        if (Time.time - startStateTime <= comboTime)
        {
            if (player.Inputs.Player.NormalAttack.triggered)
            {
                isComboInput = true;
                
            }
        }
        
        if (player.Inputs.Player.SpecialAttack.triggered)
        {
            player.isLookLocked = false;
            player.ChangeState<SpecialAttackState>();
            return;
        }
        
        if (player.Inputs.Player.Dodge.triggered)
        {
            player.ChangeState<DodgeState>();
            return;
        }
        if (player.Inputs.Player.Parry.triggered)
        {
            player.ChangeState<StartParryState>();
            return;
        }
    }

    public override void LogicUpdate(PlayerController player)
    {
        Debug.Log("Normal Attack State");
        // Normalization Time = 1이면 => 애니메이션이 끝난 상태
        // 애니메이션이 끝날 때까지 입력이 없으면 Idle로 넘어감
        // 입력이 있으면 (0.5초 이내로) 다시 NormalAttackState로 변경
        
        // 현재 진행 중인 애니메이션이 NormalAttack_(현재번호) 일 때
        animRunningTime += Time.deltaTime;
        if (Time.time - startStateTime > startAttackTime)
        {
            if (player.Attack.AttackCount >= player.Attack.MaxAttackCount)
            {
                player.Animator.ClearInt();
                player.ChangeState<IdleState>();
                return;
            }
            
            AnimatorStateInfo curAnimInfo = player.Animator.animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextAnimInfo = player.Animator.animator.GetNextAnimatorStateInfo(0);

            if (curAnimInfo.IsTag("NormalAttack"))
            { 
                float animTime = curAnimInfo.normalizedTime;

                if (animRunningTime >= attackAnimationLength)
                {
                    // 애니메이션 끝
                    if (isComboInput)
                    {
                        player.Attack.AttackCount++;
                        player.Animator.SetIntAniamtion(PlayerAnimID.NormalAttackCount, player.Attack.AttackCount);
                        player.ChangeState<NormalAttackState>();
                    }
                    else
                    {
                        player.Animator.ClearInt();
                        player.ChangeState<IdleState>();
                    }
                }
            }
            else if (nextAnimInfo.IsTag("NormalAttack"))
            {
                return;
            }
            else
            {
                player.Animator.ClearInt();
                player.ChangeState<IdleState>();
            }
        }
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
        isComboInput = false;
        player.Condition.canStaminaRecovery.Value = true;
    }
}
