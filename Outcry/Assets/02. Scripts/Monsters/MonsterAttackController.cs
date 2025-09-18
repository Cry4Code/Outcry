using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

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

    public void Test(float x, float y, GameObject projectile)
    {
        GameObject go = Instantiate(projectile, this.transform);

        //go.transform.position.x = x;
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
