using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class PlayerCondition : MonoBehaviour, IDamagable
{
    [Header("Stat Settings")]
    public Condition health;
    public Condition stamina;
    public float recoveryStaminaTime; // 스태미나 회복 주기 (일단은 1초)

    public event Action onTakeDamage;

    private float lastRecoveryStamina;

    // Start is called before the first frame update
    void Start()
    {
        health.Init(EventBusKey.ChangeHealth);
        stamina.Init(EventBusKey.ChangeStamina);
        lastRecoveryStamina = Time.time;
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
        Debug.Log("플레이어 데미지 받음");
        onTakeDamage?.Invoke();
    }

    public void Die()
    {
        Debug.Log("죽음!");
    }
}
