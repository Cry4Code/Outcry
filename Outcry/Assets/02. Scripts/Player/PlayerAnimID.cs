using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerAnimID
{
    // Bool 파라미터
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int Move = Animator.StringToHash("Move");
    public static readonly int Fall = Animator.StringToHash("Fall");
    public static readonly int WallHold = Animator.StringToHash("WallHold");

    // Trigger 파라미터
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int DoubleJump = Animator.StringToHash("DoubleJump");
    public static readonly int WallJump = Animator.StringToHash("WallJump");
    public static readonly int NormalAttack = Animator.StringToHash("NormalAttack");
    public static readonly int DownAttack = Animator.StringToHash("DownAttack");
    
    // Int 파라미터
    public static readonly int NormalAttackCount = Animator.StringToHash("NormalAttackCount");
}
