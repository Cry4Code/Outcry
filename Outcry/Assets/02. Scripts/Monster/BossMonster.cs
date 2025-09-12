using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region 임시 데이터 클래스

public static class Temp_DataTableManager
{
    public static List<MonsterSkillDataBase> monsterSkillDatas = new List<MonsterSkillDataBase>()
    {
        new MonsterSkillDataBase(1, "MonsterSkill1"),
        new MonsterSkillDataBase(2, "MonsterSkill2")
    };
}

#endregion
public class BossMonster : MonsterBase
{
    [Header("Data")]
    private List<MonsterSkillDataBase> specialSkillDatas;
    private List<MonsterSkillDataBase> commonSkillDatas;
    
    [Header("Components")]
    private MonsterCondition condition;
    private BossMonsterAI bossMonsterAI;
    private Animator animator;
    
    private void Awake()
    {
        /*condition = GetComponent<MonsterCondition>();
        bossMonsterAI = GetComponent<BossMonsterAI>();
        if (monsterData != null)
        {
            Initialize(monsterData);
            condition.Initialized(monsterData);
        }*/
    }
    
    public override void Initialize(MonsterDataBase monsterData)
    {
        /*this.monsterData = monsterData;
        
        //컨디션 초기화
        if (condition == null)
        {
            condition = this.gameObject.AddComponent<MonsterCondition>();
        }
        
        //스킬 데이터 초기화 (스페셜, 커먼)
        InitializeSkills();
        
        //몬스터 AI 초기화 (루트 노드 빌드)
        if (bossMonsterAI == null)
        {
            bossMonsterAI = this.gameObject.AddComponent<BossMonsterAI>();
            bossMonsterAI.Initialize(monsterData);
        }*/
    }

    protected override void InitializeSkills()
    {
        /*//스페셜 스킬 데이터 초기화
        foreach(int skillId in monsterData.specialSkillIds)
        {
            MonsterSkillData skillData = Temp_DataTableManager.monsterSkillDatas.Find(x => x.GetSkillId() == skillId);
            if (skillData != null)
            {
                specialSkillDatas.Add(skillData);
            }
        }
        
        //커먼 스킬 데이터 초기화
        foreach (int skillId in monsterData.commonSkillIds)
        {
            MonsterSkillData skillData = Temp_DataTableManager.monsterSkillDatas.Find(x => x.GetSkillId() == skillId);
            if (skillData != null)
            {
                commonSkillDatas.Add(skillData);
            }
        }*/
    }
}
