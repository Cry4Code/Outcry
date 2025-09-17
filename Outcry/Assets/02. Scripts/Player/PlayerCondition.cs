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

    [Header("Invisible Settings")] 
    private bool isInvinsible;
    public float invisibleTime; // 한 대 맞았을 때 무적 초 (일단은 1초)
    private WaitForSecondsRealtime waitInvisible;
    public event Action onTakeDamage;

    private float lastRecoveryStamina;

    // Start is called before the first frame update
    void Start()
    {
        health.Init(EventBusKey.ChangeHealth);
        stamina.Init(EventBusKey.ChangeStamina);
        lastRecoveryStamina = Time.time;
        invisibleTime = 1f;
        waitInvisible = new WaitForSecondsRealtime(invisibleTime);
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
        if (!isInvinsible)
        {
            Debug.Log("플레이어 데미지 받음");
            onTakeDamage?.Invoke();
            StartCoroutine(Invinsible());
        }
    }

    IEnumerator Invinsible()
    {
        isInvinsible = true;
        yield return waitInvisible;
        isInvinsible = false;
    }

    IEnumerator Invinsible(float time)
    {
        isInvinsible = true;
        yield return new WaitForSecondsRealtime(time);
        isInvinsible = false;
    }

    public void Die()
    {
        Debug.Log("죽음!");
    }
}
