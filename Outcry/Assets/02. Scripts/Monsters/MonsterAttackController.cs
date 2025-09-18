using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterAttackController : MonoBehaviour
{
    private MonsterBase monster;
    [SerializeField] private LayerMask playerLayer;

    private int damage = 1;
    
    private void Start()
    {
        monster = GetComponentInParent<MonsterBase>();
        if (monster == null)
        {
            Debug.LogError("MonsterAttackController: MonsterBase component not found!");
            return;
        }
        playerLayer = LayerMask.GetMask("Player");
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    // 투사체 생성 메서드
    // 투사체 파괴는 각 투사체가 자체적으로 함.
    public void InstantiateProjectile(GameObject projectile, Vector3 localPos)
    {
        // 부모(몬스터) 자식으로 투사체 생성
        GameObject go = Instantiate(projectile, this.transform);
        // 로컬 좌표로 위치 수정
        go.transform.localPosition = localPos;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("OnTriggerStay2D: " + other.gameObject.name);
        
        if ((playerLayer.value & (1 << other.gameObject.layer)) != 0) //(other.gameObject.layer == playerLayer)
        {
            Debug.Log("Playerlayer hit");
            Player damagable = other.gameObject.GetComponentInParent<Player>();
            if (damagable != null && damage > 0)
            {
                //todo. Player IDamagable 구현 후 데미지 주기
                // damagable.TakeDamage(damage);
                Debug.Log("Player took " + damage + " damage from " + monster.MonsterData.monsterId);
            }
        }
    }

}
