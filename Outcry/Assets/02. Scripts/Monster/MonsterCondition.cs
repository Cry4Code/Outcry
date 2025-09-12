using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCondition : MonoBehaviour /*, IDamagable*/
{
    private MonsterBase monster;
    
    private int maxHealth;
    private int currentHealth;
    private bool isDead = false;

    private void Start()
    {
        monster = GetComponent<MonsterBase>();
        if (monster == null)
        {
            Debug.LogError("BossMonsterAI: BossMonster component not found!");
            return;
        }

        Initialized();
    }

    public void Initialized()
    {
        // SetMaxHealth(monster.health);
    }

    public void SetMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
        }
    }
}
