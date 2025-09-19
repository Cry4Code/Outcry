using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterSkillModel
{
    public int skillId;
    public string skillName;
    public int damage1;
    public int damage2;
    public int damage3;
    public int healAmount;
    public float cooldown;
    public float range;
    public float triggerHealth;
    public string description;
    
    public MonsterSkillModel(int skillId, string skillName, int damage1, int damage2, int damage3, int healAmount, float cooldown, float range, float triggerHealth, string description)
    {
        this.skillId = skillId;
        this.skillName = skillName;
        this.damage1 = damage1;
        this.damage2 = damage2;
        this.damage3 = damage3;
        this.healAmount = healAmount;
        this.cooldown = cooldown;
        this.range = range;
        this.triggerHealth = triggerHealth;
        this.description = description;
    }

    public MonsterSkillModel(MonsterSkillModel monsterSkillModel)
    {
        this.skillId = monsterSkillModel.skillId;
        this.skillName = monsterSkillModel.skillName;
        this.damage1 = monsterSkillModel.damage1;
        this.damage2 = monsterSkillModel.damage2;
        this.damage3 = monsterSkillModel.damage3;
        this.healAmount = monsterSkillModel.healAmount;
        this.cooldown = monsterSkillModel.cooldown;
        this.range = monsterSkillModel.range;
        this.triggerHealth = monsterSkillModel.triggerHealth;
        this.description = monsterSkillModel.description;
    }
}
