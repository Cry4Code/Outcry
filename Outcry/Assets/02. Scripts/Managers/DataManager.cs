using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데이터베이스들 보관 및 반환 담당
/// </summary>\
[Serializable]
public class DataManager : Singleton<DataManager>
{
    [SerializeField] private SkillSequenceNodeDataList skillSequenceNodeDataList;
    [SerializeField] private MonsterSkillDataList monsterSkillDataList;
    public SkillSequenceNodeDataList SkillSequenceNodeDataList => skillSequenceNodeDataList;
    public MonsterSkillDataList MonsterSkillDataList => monsterSkillDataList;
    
    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        //MonsterSkill 리스트 초기화
        monsterSkillDataList = new MonsterSkillDataList();
        monsterSkillDataList.InitializeWithDataList(TableDataHandler.LoadMonsterSkillData());
        // SetMonsterSkillDataList();
        
        //SkillNode 리스트 초기화
        skillSequenceNodeDataList = new SkillSequenceNodeDataList();    
        skillSequenceNodeDataList.Initialize();
    }
    
    // private void SetMonsterSkillDataList()
    // {
    //     monsterSkillDataList.AddToList(new MonsterSkillModel(1, "MonsterSkill1", 1,0,0,0, 1f,10f, "첫번째 스킬입니다."));
    //     monsterSkillDataList.AddToList(new MonsterSkillModel(2, "MonsterSkill2",2, 0,0,0, 1f,10f, "두번째 스킬입니다."));
    //     monsterSkillDataList.AddToList(new MonsterSkillModel(3, "MonsterSkill3",2, 0,0,0, 1f,10f, "세번째 스킬입니다."));
    //     monsterSkillDataList.AddToList(new MonsterSkillModel(4, "MonsterSkill4",2, 0,0,0, 1f,10f,  "네번째 스킬입니다."));
    //     monsterSkillDataList.AddToList(new MonsterSkillModel(5, "MonsterSkill5",2, 0,0,0, 1f,10f,  "다섯번째 스킬입니다."));
    // }
}
