using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCondition : MonoBehaviour /*, IDamagable*/
{
    private int maxHealth;
    private int currentHealth;
    private bool isDead = false;

    public void Initialized()
    {
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
