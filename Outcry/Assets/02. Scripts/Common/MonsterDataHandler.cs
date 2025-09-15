
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
public class MonsterDataHandler
{
    /// <summary>
    /// 기획테이블에서 가져온 데이터를 현재 데이터 형식에 맞게 깊은 복사하여 맵핑
    /// </summary>
    public BossMonsterModel MapFromTableData(MonsterTableData tableData)
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
    //todo. 몬스터스킬데이터를 기획테이블에서 가져와서 맵핑하는 메서드 추가하기.
}
