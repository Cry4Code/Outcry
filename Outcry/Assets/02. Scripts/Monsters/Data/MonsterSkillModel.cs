using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterSkillModel
{
    public int skillId;
    public string skillName;
    public int damage;
    public float delay;
    public string description;
    
    public MonsterSkillModel(int skillId, string skillName, int damage, float delay, string description)
    {
        this.skillId = skillId;
        this.skillName = skillName;
        this.damage = damage;
        this.delay = delay;
        this.description = description;
    }
}
