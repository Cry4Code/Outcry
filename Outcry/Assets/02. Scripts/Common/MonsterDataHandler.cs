
#region 임시

using System;

public class MonsterTableData{ //테이블에서 받아온 데이터. (가공전)
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
public class MonsterDataHandler
{
    /// <summary>
    /// 기획테이블에서 가져온 데이터를 현재 데이터 형식에 맞게 깊은 복사하여 맵핑
    /// </summary>
    public BossMonsterModel MapFromTableData(MonsterTableData tableData)
    {   
        //todo think. deepcopy를 굳이 해줘야하나?
        
        //
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
}
