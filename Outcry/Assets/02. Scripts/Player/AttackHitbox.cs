using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private Player Player;
    [field : SerializeField] public int Damage { get; set; }

    public void Init(Player player)
    {
        Player = player;
        Damage = 10;
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Player.PlayerAttack.isStartParry)
        {
            Player.PlayerAttack.successParry = true;

            if (other.TryGetComponent(out ICountable countable))
            {
                Player.PlayerCondition.SetInvincible(0.2f);
                countable?.CounterAttacked();
                Debug.Log("플레이어 패링 성공");
            }
            return;
        }
            
        if (other.TryGetComponent<IDamagable>(out var damagable))
            // && other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            damagable?.TakeDamage(Damage);
            Debug.Log($"플레이어가 몬스터에게 {Damage} 만큼 데미지 줌");
        } 
        
    }
}
