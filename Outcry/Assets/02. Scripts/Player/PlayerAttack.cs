using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    #region 기본 공격 관련

    [field: Header("Normal Attack")] 
    public int AttackCount = 0;

    public bool HasJumpAttack = false;
    public int MaxAttackCount = 2;

    
    #endregion


    public void NormalAttack()
    {
        Debug.Log($"{AttackCount + 1} 번째 공격");
    }

    public void Update()
    {
        Debug.Log($"어택 카운트 : {AttackCount}");
    }

    public void ClearAttackCount()
    {
        AttackCount = 0;
        HasJumpAttack = false;
    }
}
