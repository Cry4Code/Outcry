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
/// </summary>
public static class TableDataHandler
{
    // -GetExampleData(): ExampleModel static 
    // +GetExampleDatabase(): List<ExampleModel> static
    public static BossMonsterModel MapFromTableData(MonsterTableData tableData)
    {   
        // deep copy 이유: 해당 모델 인스펙터에서 보여주기 위해 public 인자를 갖고 있음.
        // 데이터 변경이 되더라도 해당 객체의 데이터만 변경될 것임.
        int[] specialSkillIds = new int[tableData.specialSkillIds.Length];
        Array.Copy(tableData.specialSkillIds, specialSkillIds, specialSkillIds.Length);
        int[] commonSkillIds = new int[tableData.commonSkillIds.Length];
        Array.Copy(tableData.commonSkillIds, commonSkillIds, commonSkillIds.Length);
        
        BossMonsterModel newBossMonsterModel = new BossMonsterModel(
            tableData.monsterId, tableData.monsterName, tableData.health,
            tableData.chaseSpeed, tableData.attackRange, tableData.attackCooldown,
            specialSkillIds,
            commonSkillIds);

        return newBossMonsterModel;
    }

    public static void LoadMonsterData()
    {
        DataTableManager.Instance.LoadCollectionData<EnemyDataTable>();
        DataTableManager.Instance.GetCollectionData<EnemyData>();
        
    }
    //todo. 몬스터스킬데이터를 기획테이블에서 가져와서 맵핑하는 메서드 추가하기.
}
