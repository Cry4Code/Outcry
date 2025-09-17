using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterModelBase
{
    public int monsterId;
    public string monsterName;
    public int health;
    public float chaseSpeed;
    public float attackRange;
    public float attackCooldown;
    
    public MonsterModelBase(
        int monsterId, string monsterName, int health, float chaseSpeed, float attackRange, float attackCooldown)
    {
        this.monsterId = monsterId;
        this.monsterName = monsterName;
        this.health = health;
        this.chaseSpeed = chaseSpeed;
        this.attackRange = attackRange;
        this.attackCooldown = attackCooldown;
    }
}