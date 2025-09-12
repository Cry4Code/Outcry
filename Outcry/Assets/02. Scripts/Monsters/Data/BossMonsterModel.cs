using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterModel : MonsterModelBase   //주원님 이거 readonly로 생성해주세요...
{
    public readonly int[] specialSkillIds;
    public readonly int[] commonSkillIds;

    public BossMonsterModel(
        int monsterId, string monsterName, int health, float chaseSpeed, float attackRange, float attackCooldown, 
        int[] specialSkillIds, int[] commonSkillIds) : 
        base(monsterId, monsterName, health, chaseSpeed, attackRange, attackCooldown)
    {
        this.specialSkillIds = specialSkillIds;
        this.commonSkillIds = commonSkillIds;
    }
}
