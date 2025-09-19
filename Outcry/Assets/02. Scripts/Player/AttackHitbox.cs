using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private PlayerController controller;
    [field : SerializeField] public int Damage { get; set; }

    public void Init(PlayerController player)
    {
        controller = player;
        Damage = 10;
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (controller.Attack.isStartParry)
        {
            if (other.TryGetComponent(out ICountable countable))
            {
                controller.Attack.successParry = true;
                countable?.CounterAttacked();
                Debug.Log("[플레이어] 플레이어 패링 성공");
            }
            return;
        }
            
        if (other.TryGetComponent<IDamagable>(out var damagable))
            // && other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            damagable?.TakeDamage(Damage);
            Debug.Log($"[플레이어] 플레이어가 몬스터에게 {Damage} 만큼 데미지 줌");
        } 
        
    }
}
