using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BossMonster : MonsterBase
{
    [Header("Data")]
    [SerializeField] private List<MonsterSkillModel> specialSkillDatas;
    [SerializeField] private List<MonsterSkillModel> commonSkillDatas;

    protected override void InitializeSkills()
    {
        if (DataManager.Instance == null)
            Debug.LogError("DataManager.Instance가 null입니다.");
        else if (DataManager.Instance.MonsterSkillDataList == null)
            Debug.LogError("MonsterSkillDataList가 null입니다.");
        specialSkillDatas = new List<MonsterSkillModel>();
        commonSkillDatas = new List<MonsterSkillModel>();
        
        if (monsterData is BossMonsterModel bossMonsterData)
        {
            Debug.Log("BossMonster임");
            //스페셜 스킬 데이터 초기화
            foreach (int skillId in bossMonsterData.specialSkillIds)
            {
                
                DataManager.Instance.MonsterSkillDataList.TryGetMonsterSkillModelData(skillId, out MonsterSkillModel skillData);
                if (skillData != null)
                {
                    specialSkillDatas.Add(skillData);
                }
            }

            //커먼 스킬 데이터 초기화
            foreach (int skillId in bossMonsterData.commonSkillIds)
            {
                DataManager.Instance.MonsterSkillDataList.TryGetMonsterSkillModelData(skillId, out MonsterSkillModel skillData);
                if (skillData != null)
                {
                    commonSkillDatas.Add(skillData);
                }
            }
        }
    }
}
