using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CommonMonsterModel : MonsterModelBase
{
    public int[] commonSkillsIds;

    public CommonMonsterModel(
        int monsterId, string monsterName, int health, float chaseSpeed, float detectRange, float attackCooldown,
        int[] commonSkillIds) :
        base(monsterId, monsterName, health, chaseSpeed, detectRange, attackCooldown)
    {
        this.commonSkillsIds = commonSkillIds;
    }
}
