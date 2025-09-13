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
        new MonsterSkillModel(2, "MonsterSkill2",2, 2f, "두번째 스킬입니다.")
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

    protected void Awake()
    {
        //test용 코드
        this.monsterData = new BossMonsterModel(
            1, "BossMonster1", 100,
            10f, 10f, 10f, new int[2] {1,1}, new int[2] {2,2});
        Debug.Log($"{monsterData.monsterId}가 Awake 되었습니다.");
        base.Awake();
    }

    protected void Update()
    {
        monsterAI.UpdateAI();
    }

    public override void Initialize()
    {
        Debug.Log($"Initialize: {monsterData.monsterId}");
        monsterAI.InitializeBehaviorTree();
    }
    
    // public override void Initialize(MonsterModelBase monsterData)   //외부에서 호출하는 Initializer
    // {
    //     this.monsterData = monsterData;
    //     if (monsterData != null)    //
    //         Initialize();
    // }
    //
    // private void Initialize()   //내부 호출용 Initializer
    // {
    //     if(condition == null)
    //     {
    //         condition = this.AddComponent<MonsterCondition>();
    //     }
    //     if (monsterData != null)
    //     {
    //         condition.Initialize();
    //         InitializeSkills();
    //         if(monsterAI == null)
    //         {
    //             monsterAI = this.AddComponent<BossMonsterAI>();
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("monsterData is null");
    //     }
    // }

    protected override void InitializeSkills()
    {
        if (monsterData is BossMonsterModel bossMonsterData)
        {
            //스페셜 스킬 데이터 초기화
            foreach (int skillId in bossMonsterData.specialSkillIds)
            {
                MonsterSkillModel skillData =
                    Temp_DataBase.GetMonsterSkillById(bossMonsterData.specialSkillIds[skillId]);
                if (skillData != null)
                {
                    specialSkillDatas.Add(skillData);
                }
            }

            //커먼 스킬 데이터 초기화
            foreach (int skillId in bossMonsterData.commonSkillIds)
            {
                MonsterSkillModel skillData =
                    Temp_DataBase.GetMonsterSkillById(bossMonsterData.commonSkillIds[skillId]);
                if (skillData != null)
                {
                    specialSkillDatas.Add(skillData);
                }
            }
        }
    }
}
