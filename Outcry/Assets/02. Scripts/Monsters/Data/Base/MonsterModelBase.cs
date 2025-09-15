using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract 클래스라고 생각하고 new로 생성하지 말 것.
/// </summary>
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