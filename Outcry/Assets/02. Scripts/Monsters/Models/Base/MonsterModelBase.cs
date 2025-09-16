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
    public float detectRange;
    public float attackCooldown;
    
    public MonsterModelBase(
        int monsterId, string monsterName, int health, float chaseSpeed, float detectRange, float attackCooldown)
    {
        this.monsterId = monsterId;
        this.monsterName = monsterName;
        this.health = health;
        this.chaseSpeed = chaseSpeed;
        this.detectRange = detectRange;
        this.attackCooldown = attackCooldown;
    }
}