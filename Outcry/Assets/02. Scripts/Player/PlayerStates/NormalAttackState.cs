using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackState : IPlayerState
{
    private float startStateTime;
    private float startAttackTime = 0.01f;
    private float comboTime = 0.5f; // 콤보타임 지나서 누르면 의미없음.
    private bool isComboInput = false;
    
    
    public void Enter(PlayerController player)
    {
        startStateTime = Time.time;
        isComboInput = false;
        // AttackCount = 0 + NormalAttack Trigger On.
        player.PlayerAnimator.ClearBool();
        player.PlayerAnimator.SetTriggerAnimation(PlayerAnimID.NormalAttack);
        player.Inputs.Player.Move.Disable();
        
    }

    public void HandleInput(PlayerController player)
    {
        // 키 입력이 필요
        if (Time.time - startStateTime <= comboTime)
        {
            if (player.Inputs.Player.NormalAttack.triggered)
            {
                isComboInput = true;
                
            }
        }
    }

    public void LogicUpdate(PlayerController player)
    {
        // Normalization Time = 1이면 => 애니메이션이 끝난 상태
        // 애니메이션이 끝날 때까지 입력이 없으면 Idle로 넘어감
        // 입력이 있으면 (0.5초 이내로) 다시 NormalAttackState로 변경
        
        // 현재 진행 중인 애니메이션이 NormalAttack_(현재번호) 일 때
        if (Time.time - startStateTime > startAttackTime)
        {
            AnimatorStateInfo curAnimInfo = player.PlayerAnimator.animator.GetCurrentAnimatorStateInfo(0);

            if (curAnimInfo.IsName($"NormalAttack_{player.PlayerAttack.AttackCount+1}"))
            { 
                float animTime = curAnimInfo.normalizedTime;

                if (animTime >= 1.0f)
                {
                    if (player.PlayerAttack.AttackCount >= player.PlayerAttack.MaxAttackCount)
                    {
                        player.PlayerAnimator.ClearInt();
                        player.ChangeState<IdleState>();
                        return;
                    }
                    // 애니메이션 끝났음.
                    if (isComboInput)
                    {
                        player.PlayerAttack.AttackCount++;
                        player.PlayerAnimator.SetIntAniamtion(PlayerAnimID.NormalAttackCount, player.PlayerAttack.AttackCount);
                        player.ChangeState<NormalAttackState>();
                        return;
                    }

                    player.ChangeState<IdleState>();
                    return;
                }
            }
        }
        
    }

    public void Exit(PlayerController player)
    {
        isComboInput = false;
    }
}
