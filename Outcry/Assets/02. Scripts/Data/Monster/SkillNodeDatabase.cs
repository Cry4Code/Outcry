using System;
using System.Collections.Generic;
using System.Linq;
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


public static class SkillNodeDatabase   //todo. think. 데이터베이스 진짜 관리 어케할거임...?
{    
    //초기화및인스펙터용 List와 런타임용 Dictionary를 분리하였으므로, 동기화에 주의할것.
    public static List<SkillNode> skillNodes = new List<SkillNode>();
    private static Dictionary<int, SkillSequenceNode> skillNodeDict;    //런타임용 딕셔너리

    static SkillNodeDatabase()
    {
        Initialize();
    }
    public static void Initialize()
    {
        //데이터 초기화. todo. 지금은 임시이므로 기획테이블에서 가져오는 것으로 변경되어야함.
        skillNodes.Clear();
        skillNodes.Add(new SkillNode() { skillId = 1, skillNode = new TestSkillSequenceNode() });
        skillNodes.Add(new SkillNode() { skillId = 2, skillNode = new TestSkillSequenceNode() });
        
        //런타임 사용을 위한 딕셔너리 생성
        skillNodeDict = skillNodes.ToDictionary(x => x.skillId, x => x.skillNode);
    }

    public static SkillSequenceNode GetSkillNode(int id)
    {
        if (skillNodeDict != null && skillNodeDict.TryGetValue(id, out SkillSequenceNode skillNode))
        {
            return skillNode;
        }

        Debug.LogError($"id {id}에 해당하는 SkillSequenceNode를 찾을 수 없음");
        return null;
    }
    public static List<SkillSequenceNode> GetSkillNodes(int[] ids)
    {
        List<SkillSequenceNode> skills = new List<SkillSequenceNode>();
        foreach (int id in ids)
        {
            SkillSequenceNode skill = GetSkillNode(id);
            if (skill != null)
            {
                skills.Add(skill);
            }
        }

        return skills;
    }
}