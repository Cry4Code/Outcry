using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillModel
{
    public readonly int skillId;
    public readonly string skillName;
    public readonly int damage;
    public readonly float delay;
    public readonly string description;
    
    public MonsterSkillModel(int skillId, string skillName, int damage, float delay, string description)
    {
        this.skillId = skillId;
        this.skillName = skillName;
        this.damage = damage;
        this.delay = delay;
        this.description = description;
    }
}
