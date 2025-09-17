using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
#region 임시

using System;

public class MonsterTableData{ //테이블에서 받아온 데이터. (가공전: 추후 기획테이블에서 자동생성될 예정)
    public int monsterId;
    public string monsterName;
    public int health;
    public float chaseSpeed;
    public float attackRange;
    public float attackCooldown;
    public int[] specialSkillIds;
    public int[] commonSkillIds;
}

#endregion

/// <summary>
/// 순수 변환만을 담당함 (기획테이블 데이터 -> 모델)
/// DataManager에서 Initialize할때 호출됨
/// 데이터를 보관하지 않으며, 가공하는 메서드만 존재.
/// 추가로 필요한 변환 메서드는 본인의 원하는 형태로 구현.
/// </summary>
public static class TableDataHandler
{
    public static List<MonsterSkillModel> LoadMonsterSkillData()
    {
        List<MonsterSkillModel> monsterSkillDataList = new List<MonsterSkillModel>();
        
        // tableData: json에서 불러온 데이터
        DataTableManager.Instance.LoadCollectionData<EnemySkillDataTable>();
        Dictionary<int, IData> tableData = DataTableManager.Instance.CollectionData[typeof(EnemySkillData)] as Dictionary<int, IData>;
        
        // tableData의 각 아이템을 MonsterSkillModel로 변환하여 리스트에 추가
        foreach (var item in tableData.Values)
        {
            MonsterSkillModel monsterSkillData = MapFromTableData(item as EnemySkillData);
            monsterSkillDataList.Add(monsterSkillData);
            Debug.Log($"{monsterSkillData.skillId} : {monsterSkillData.skillName}");
        }

        return monsterSkillDataList;
    }
    private static MonsterSkillModel MapFromTableData(EnemySkillData tableData)
    {
        MonsterSkillModel newMonsterSkillModel = new MonsterSkillModel(
            tableData.Skill_id, tableData.Skill_name, 
            tableData.Damage, tableData.Damage2, tableData.Damage3, tableData.HealAmount, 
            tableData.Cooldown, tableData.Range, tableData.Desc);
        
        return newMonsterSkillModel;
    }
    
    //todo. 몬스터스킬데이터를 기획테이블에서 가져와서 맵핑하는 메서드 추가하기.
}
