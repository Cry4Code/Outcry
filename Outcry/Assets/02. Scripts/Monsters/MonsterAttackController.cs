using System;
using System.Collections;
using System.Collections.Generic;
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

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("OnTriggerStay2D: " + other.gameObject.name);
        
        if ((playerLayer.value & (1 << other.gameObject.layer)) != 0) //(other.gameObject.layer == playerLayer)
        {
            Debug.Log("Playerlayer hit");
            Player damagable = other.gameObject.GetComponentInParent<Player>();
            if (damagable != null && damage > 0)
            {
                // damagable.TakeDamage(damage);
                Debug.Log("Player took " + damage + " damage from " + monster.MonsterData.monsterId);
            }
        }
    }
}
