using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [HideInInspector] public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 모든 bool 끄고 특정 bool만 켜기
    /// </summary>
    public void SetBoolAnimation(int animHash)
    {
        ClearBool();

        animator.SetBool(animHash, true);
    }

    /// <summary>
    /// 트리거 애니메이션 실행 (중복 방지를 위해 Reset 후 Set)
    /// </summary>
    public void SetTriggerAnimation(int animHash)
    {
        ClearTrigger();

        animator.SetTrigger(animHash);
    }

    /// <summary>
    /// Int 값 수정
    /// </summary>
    /// <param name="animHash"></param>
    /// <param name="value"></param>
    public void SetIntAniamtion(int animHash, int value)
    {
        animator.SetInteger(animHash, value);
    }
    public void ClearBool()
    {
        animator.SetBool(PlayerAnimID.Idle, false);
        animator.SetBool(PlayerAnimID.Move, false);
        animator.SetBool(PlayerAnimID.Fall, false);
        animator.SetBool(PlayerAnimID.WallHold, false);
    }

    public void ClearTrigger()
    {
        animator.ResetTrigger(PlayerAnimID.Jump);
        animator.ResetTrigger(PlayerAnimID.DoubleJump);
        animator.ResetTrigger(PlayerAnimID.WallJump);
        animator.ResetTrigger(PlayerAnimID.NormalAttack);
    }

    public void ClearInt()
    {
        animator.SetInteger(PlayerAnimID.NormalAttackCount, 0);
    }

}
