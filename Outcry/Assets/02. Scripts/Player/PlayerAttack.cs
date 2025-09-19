using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    #region 기본 공격 관련

    [field: Header("Normal Attack")] 
    public int AttackCount = 0;
    public int MaxAttackCount = 2;
    public bool HasJumpAttack = false;

    #endregion
    
    #region 패링 관련

    [field: Header("Parry")] 
    public bool isStartParry = false;
    public bool successParry = false;
    
    
    #endregion
    public void Update()
    {
        Debug.Log($"[플레이어] 어택 카운트 : {AttackCount}");
    }

    public void ClearAttackCount()
    {
        AttackCount = 0;
    }
}
