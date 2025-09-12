using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillDataBase
{
    private int skillId;
    private string skillName;
    private int damage;
    private float delay;
    private string description;
    
    //임시 생성자
    public MonsterSkillDataBase(int skillId, string skillName)
    {
        this.skillId = skillId;
        this.skillName = skillName;
    }
    //임시 접근
    public int GetSkillId() { return skillId; }
}
