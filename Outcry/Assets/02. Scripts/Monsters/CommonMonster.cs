using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonMonster : MonsterBase
{
    [Header("Data")]
    [SerializeField] private List<MonsterSkillModel> commonSkillDatas;

    // 페트롤 시간, 이동 속도 등 데이터 추가? 미정
    // private float patrolSpeed;

    protected override void InitializeSkills()
    {
        // 데이터메니저 null 체크
        if (DataManager.Instance == null)
            Debug.LogError("DataManager.Instance가 null입니다.");
        else if (DataManager.Instance.MonsterSkillDataList == null)
            Debug.LogError("MonsterSkillDataList가 null입니다.");

        commonSkillDatas = new List<MonsterSkillModel>();

        // 몬스터 데이터가 일반 몬스터면
        if (monsterData is CommonMonsterModel commonMonsterData)
        {
            Debug.Log("CommonMonster임");

            // 일반 스킬 데이터 초기화
            foreach (int skillId in commonMonsterData.commonSkillsIds)
            {
                DataManager.Instance.MonsterSkillDataList.GetMonsterSkillModelData(skillId, out MonsterSkillModel skillData);
                if (skillData != null)
                    commonSkillDatas.Add(skillData);                
            }
        }
    }
}
