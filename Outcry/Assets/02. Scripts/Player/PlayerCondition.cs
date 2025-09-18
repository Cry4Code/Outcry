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

    [Header("Invisible Settings")] 
    private bool isInvincible;
    public float invincibleTime; // 한 대 맞았을 때 무적 초 (일단은 1초)
    private WaitForSecondsRealtime waitInvisible;
    public event Action onTakeDamage;

    private float lastRecoveryStamina;
    private PlayerController player;
    private Coroutine invincibleCoroutine;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        health.Init(EventBusKey.ChangeHealth);
        stamina.Init(EventBusKey.ChangeStamina);
        lastRecoveryStamina = Time.time;
        invincibleTime = 0.75f;
        waitInvisible = new WaitForSecondsRealtime(invincibleTime);
        invincibleCoroutine = null;
    }

    void Update()
    {
        if(Time.time - lastRecoveryStamina >= recoveryStaminaTime)
        {
            stamina.Add(stamina.passiveValue);
            Debug.Log($"Stamina : {stamina.CurValue()}");
            lastRecoveryStamina = Time.time;
        }

        if (health.CurValue() <= 0f)
        {
            Die();
        }
    }



    public void TakeDamage(int damage)
    {
        if (player.PlayerAttack.successParry)
        {
            if (invincibleCoroutine != null) return;
            invincibleCoroutine = StartCoroutine(Invincible(0.3f));
        }
        if (invincibleCoroutine == null)
        {
            if(!player.IsCurrentState<DamagedState>()) player.ChangeState<DamagedState>();
            invincibleCoroutine = StartCoroutine(Invincible());
            Debug.Log("플레이어 데미지 받음");
            health.Substract(damage);
            Debug.Log($"플레이어 현재 체력 : {health.CurValue()}");
            onTakeDamage?.Invoke();
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
        Debug.Log("플레이어 무적 시작");
        isInvincible = true;
        yield return waitInvisible;
        isInvincible = false;
        invincibleCoroutine = null;
        Debug.Log("플레이어 무적 끝");
    }

    IEnumerator Invincible(float time)
    {
        Debug.Log("플레이어 무적 시작");
        isInvincible = true;
        yield return new WaitForSecondsRealtime(time);
        isInvincible = false;
        invincibleCoroutine = null;
        Debug.Log("플레이어 무적 끝");
    }

    public void Die()
    {
        Debug.Log("죽음!");
    }
}
