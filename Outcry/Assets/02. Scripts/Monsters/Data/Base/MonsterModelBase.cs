using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterModelBase
{
    public readonly int monsterId;
    public readonly string monsterName;
    public readonly int health;
    public readonly float chaseSpeed;
    public readonly float attackRange;
    public readonly float attackCooldown;
    
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