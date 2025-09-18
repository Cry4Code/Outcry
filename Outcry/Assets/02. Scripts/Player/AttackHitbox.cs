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
            Debug.Log("패링 성공");
            // TODO : 나중에 other.collider.TryGetComponent 해서 
            // ICountable 찾으면 그거 부르도록 할거임 
            return;
        }
        
        if (other.TryGetComponent<IDamagable>(out var damagable))
        {
            Debug.Log("플레이어가 몬스터의 IDamagable을 찾음");
        }
            
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            Debug.Log("플레이어가 몬스터의 레이어를 찾음");
        }
            
        /*if (other.TryGetComponent<IDamagable>(out var damagable)
            && other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            damagable?.TakeDamage(Damage);
            Debug.Log($"플레이어가 몬스터에게 {Damage} 만큼 데미지 줌");
        }*/ 
        
    }
}
