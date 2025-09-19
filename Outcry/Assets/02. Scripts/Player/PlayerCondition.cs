using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Serialization;

public class PlayerCondition : MonoBehaviour, IDamagable
{
    [Header("Stat Settings")]
    public Condition health;
    public Condition stamina;
    public float startRecoveryStaminaTime; // 스태미나 회복 최소 시간
    public Observable<bool> canStaminaRecovery;
    private float recoveryElapsedTime;
    private float recoveryFullTime;
    private float recoveryStaminaThresholdTime;
    private float k; // k = (최대 회복시간) / (최대 스태미나)^2
    private float reverseK; // 1/k;

    [Header("Invisible Settings")] 
    private bool isInvincible;
    public float invincibleTime; // 한 대 맞았을 때 무적 초 (일단은 1초)
    private WaitForSecondsRealtime waitInvisible;
    
    private PlayerController controller;
    private Coroutine invincibleCoroutine;
    [HideInInspector] public bool isDead;

    private void Awake()
    {
        canStaminaRecovery = new Observable<bool>(EventBusKey.ChangeStaminaRecovery, false);
        controller = GetComponent<PlayerController>();
        
        
    }

    private void OnEnable()
    {
        EventBus.Subscribe(EventBusKey.ChangeStaminaRecovery, OnStaminaRecoveryChanged);
        EventBus.Subscribe(EventBusKey.ChangeStamina, OnStaminaChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(EventBusKey.ChangeStaminaRecovery, OnStaminaRecoveryChanged);
        EventBus.Unsubscribe(EventBusKey.ChangeStamina, OnStaminaChanged);
    }

    void Start()
    {
        health.startValue = health.maxValue = controller.Data.maxHealth;
        stamina.startValue = stamina.maxValue = controller.Data.maxStamina;
        health.Init(EventBusKey.ChangeHealth);
        stamina.Init(EventBusKey.ChangeStamina);
        invincibleTime = controller.Data.invincibleTime;
        startRecoveryStaminaTime = controller.Data.rateStamina;
        recoveryFullTime = controller.Data.fullStamina;
        k = (recoveryFullTime) / Mathf.Pow(stamina.maxValue, 2);
        reverseK = 1 / k;
        Debug.Log($"[플레이어] 스태미나 상수 k = {k}");
        waitInvisible = new WaitForSecondsRealtime(invincibleTime);
        invincibleCoroutine = null;
        canStaminaRecovery.Value = true;
        isDead = false;
    }

    void FixedUpdate()
    {
        Debug.Log($"[플레이어] 스태미나 : {stamina.CurValue()}");
        if(canStaminaRecovery.Value && stamina.CurValue() < stamina.maxValue)
        {
            if (Time.time > recoveryStaminaThresholdTime)
            {
                recoveryElapsedTime += Time.fixedDeltaTime;
                var tempStamina = Mathf.FloorToInt(Mathf.Sqrt(recoveryElapsedTime * reverseK));
                stamina.SetCurValue(Mathf.Min(stamina.maxValue, tempStamina));
            }
        }

        if (health.CurValue() <= 0f && !isDead)
        {
            Die();
        }
    }



    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            if (!controller.Animator.animator.GetAnimatorTransitionInfo(0).IsName("Die"))
            {
                controller.ChangeState<DieState>();
            }
            return;
        }
        if (controller.Attack.successParry)
        {
            if (invincibleCoroutine != null) return;
            invincibleCoroutine = StartCoroutine(Invincible(controller.Data.parryInvincibleTime));
        }
        if (invincibleCoroutine == null)
        {
            if(!controller.IsCurrentState<DamagedState>()) controller.ChangeState<DamagedState>();
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
            canStaminaRecovery.Value = false;
            stamina.Substract(useStamina);
            Debug.Log($"[플레이어] 스태미나 {useStamina} 사용. 현재 스태미나 {stamina.CurValue()}");
            canStaminaRecovery.Value = true;
            return true;
        }
        else
        {
            Debug.Log($"[플레이어] 스태미나 {useStamina} 사용 불가");
            canStaminaRecovery.Value = true;
            return false;
        }
        
    }

    public void Die()
    {
        isDead = true;
        Debug.Log("[플레이어] 죽음!");
        controller.ChangeState<DieState>();
    }

    public void OnStaminaRecoveryChanged(object data)
    {
        // RecoveryChanged가 True 로 바꼈을 때
        if ((bool)data)
        {
            // Debug.Log("[플레이어] 스태미나 리커버리 켜짐");
            recoveryElapsedTime = k * Mathf.Pow(stamina.CurValue(), 2);
            recoveryStaminaThresholdTime = Time.time + startRecoveryStaminaTime; // 1초 뒤에 시작할 수 있게
            // Debug.Log($"[플레이어] 스태미나 상수 k 에 곱해진 현재 스태미나 : {Mathf.Pow(stamina.CurValue(), 2)}");
            // Debug.Log($"[플레이어] 스태미나 리커버리 시간 {recoveryElapsedTime}");
        }
        else
        {
            recoveryElapsedTime = 0;
            // Debug.Log("[플레이어] 스태미나 리커버리 꺼짐");
        }
    }

    public void OnStaminaChanged(object data)
    {
        if ((int)data < stamina.maxValue)
        {
            canStaminaRecovery.Value = true;
        }
        else
        {
            canStaminaRecovery.Value = false;
        }
    }
}
