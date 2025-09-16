using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MonsterSkillDataList : DataListBase<MonsterSkillModel>
{
    public override void Initialize()
    {
        dataList = new List<MonsterSkillModel>();
    }
    
    /// <summary>
    /// 데이터리스트에 id에 해당하는 스킬데이터(MonsterSkillModel)가 있다면 true, 없다면 false 반환
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="monsterSkillData"></param>
    /// <returns></returns>
    public bool GetSkillSequenceNode(int skillId, out MonsterSkillModel monsterSkillData)
    {
        monsterSkillData = dataList.FirstOrDefault(data => data.skillId == skillId);

        if (monsterSkillData == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
