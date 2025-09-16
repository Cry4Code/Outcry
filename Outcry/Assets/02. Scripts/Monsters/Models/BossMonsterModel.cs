using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossMonsterModel : MonsterModelBase
{
    public int[] specialSkillIds;
    public int[] commonSkillIds;

    public BossMonsterModel(
        int monsterId, string monsterName, int health, float chaseSpeed, float detectRange, float attackCooldown, 
        int[] specialSkillIds, int[] commonSkillIds) : 
        base(monsterId, monsterName, health, chaseSpeed, detectRange, attackCooldown)
    {
        this.specialSkillIds = specialSkillIds;
        this.commonSkillIds = commonSkillIds;
    }
}
