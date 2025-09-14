using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

#region 임시 데이터 클래스

public static class Temp_DataBase
{
    public static List<MonsterSkillModel> monsterSkillDatas = new List<MonsterSkillModel>()
    {
        new MonsterSkillModel(1, "MonsterSkill1", 1, 1f, "첫번째 스킬입니다."),
        new MonsterSkillModel(2, "MonsterSkill2",2, 2f, "두번째 스킬입니다."),
        new MonsterSkillModel(3, "MonsterSkill3",2, 2f, "세번째 스킬입니다."),
        new MonsterSkillModel(4, "MonsterSkill4",2, 2f, "네번째 스킬입니다."),
        new MonsterSkillModel(5, "MonsterSkill5",2, 2f, "다섯번째 스킬입니다.")
    };

    public static MonsterSkillModel GetMonsterSkillById(int id)
    {
        return monsterSkillDatas.FirstOrDefault(skill => skill.skillId == id);
    }
}

#endregion
public class BossMonster : MonsterBase
{
    [Header("Data")]
    [SerializeField] private List<MonsterSkillModel> specialSkillDatas;
    [SerializeField] private List<MonsterSkillModel> commonSkillDatas;


    protected new void Awake()
    {
        //test용 코드
        this.monsterData = new BossMonsterModel(
            1, "BossMonster1", 100,
            10f, 10f, 10f, new int[2] {1,1}, new int[2] {2,2});
        Debug.Log($"{monsterData.monsterId}가 Awake 되었습니다.");
        base.Awake();
    }

    protected override void InitializeSkills()
    {
        if (monsterData is BossMonsterModel bossMonsterData)
        {
            Debug.Log("BossMonster임");
            //스페셜 스킬 데이터 초기화
            foreach (int skillId in bossMonsterData.specialSkillIds)
            {
                MonsterSkillModel skillData =
                    Temp_DataBase.GetMonsterSkillById(skillId);
                if (skillData != null)
                {
                    specialSkillDatas.Add(skillData);
                }
            }

            //커먼 스킬 데이터 초기화
            foreach (int skillId in bossMonsterData.commonSkillIds)
            {
                MonsterSkillModel skillData =
                    Temp_DataBase.GetMonsterSkillById(skillId);
                if (skillData != null)
                {
                    commonSkillDatas.Add(skillData);
                }
            }
        }
    }
}
