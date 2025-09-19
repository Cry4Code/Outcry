using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Serialization;

public class PlayerCondition : MonoBehaviour, IDamagable
{
    [Header("Stat Settings")]
    public Condition health;
    public Condition stamina;
    public float recoveryStaminaTime; // 스태미나 회복 주기 (일단은 1초)
    public bool canStaminaRecovery;

    [Header("Invisible Settings")] 
    private bool isInvincible;
    public float invincibleTime; // 한 대 맞았을 때 무적 초 (일단은 1초)
    private WaitForSecondsRealtime waitInvisible;

    private float lastRecoveryStamina;
    private PlayerController player;
    private Coroutine invincibleCoroutine;
    [HideInInspector] public bool isDead;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }
    
    void Start()
    {
        health.Init(EventBusKey.ChangeHealth);
        stamina.Init(EventBusKey.ChangeStamina);
        lastRecoveryStamina = Time.time;
        invincibleTime = 0.75f;
        waitInvisible = new WaitForSecondsRealtime(invincibleTime);
        invincibleCoroutine = null;
        canStaminaRecovery = true;
        isDead = false;
    }

    void FixedUpdate()
    {
        if(Time.time - lastRecoveryStamina >= recoveryStaminaTime && canStaminaRecovery)
        {
            stamina.Add(stamina.passiveValue);
            Debug.Log($"[플레이어] 스태미나 : {stamina.CurValue()}");
            lastRecoveryStamina = Time.time;
        }

        if (health.CurValue() <= 0f && !isDead)
        {
            Die();
        }
    }



    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (player.PlayerAttack.successParry)
        {
            if (invincibleCoroutine != null) return;
            invincibleCoroutine = StartCoroutine(Invincible(0.3f));
        }
        if (invincibleCoroutine == null)
        {
            if(!player.IsCurrentState<DamagedState>()) player.ChangeState<DamagedState>();
            invincibleCoroutine = StartCoroutine(Invincible());
            Debug.Log("[플레이어] 플레이어 데미지 받음");
            health.Substract(damage);
            Debug.Log($"[플레이어] 플레이어 현재 체력 : {health.CurValue()}");
        }
        
    }

    public void SetInvincible(float time)
    {
        if (!isInvincible)
        {
            StartCoroutine(Invincible(time));
        }
    }

    IEnumerator Invincible()
    {
        Debug.Log("[플레이어] 플레이어 무적 시작");
        isInvincible = true;
        yield return waitInvisible;
        isInvincible = false;
        invincibleCoroutine = null;
        Debug.Log("[플레이어] 플레이어 무적 끝");
    }

    IEnumerator Invincible(float time)
    {
        Debug.Log("[플레이어] 플레이어 무적 시작");
        isInvincible = true;
        yield return new WaitForSecondsRealtime(time);
        isInvincible = false;
        invincibleCoroutine = null;
        Debug.Log("[플레이어] 플레이어 무적 끝");
    }

    public bool TryUseStamina(int useStamina)
    {
        if (stamina.CurValue() - useStamina >= 0)
        {
            stamina.Substract(useStamina);
            Debug.Log($"[플레이어] 스태미나 {useStamina} 사용. 현재 스태미나 {stamina.CurValue()}");
            return true;
        }
        else
        {
            Debug.Log($"[플레이어] 스태미나 {useStamina} 사용 불가");
            return false;
        }
    }

    public void Die()
    {
        isDead = true;
        Debug.Log("[플레이어] 죽음!");
        player.ChangeState<DieState>();
    }
}
