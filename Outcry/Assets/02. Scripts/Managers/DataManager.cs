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
    [SerializeField] private SkillNodeDataList skillNodeDataList;
    [SerializeField] private MonsterSkillDataList monsterSkillDataList;
    public SkillNodeDataList SkillNodeDataList => skillNodeDataList;
    public MonsterSkillDataList MonsterSkillDataList => monsterSkillDataList;
    
    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        
        //SkillNode 리스트 초기화
        skillNodeDataList = new SkillNodeDataList();    
        skillNodeDataList.Initialize();
        SetSkillNodeDataList();
        
        //MonsterSkill 리스트 초기화
        monsterSkillDataList = new MonsterSkillDataList();
        monsterSkillDataList.Initialize();
        SetMonsterSkillDataList();
    }
    
    
    //스킬 노em 생성 코드 todo. think. 이게 베스트일까?
    private void SetSkillNodeDataList()
    {
        skillNodeDataList.AddToList(new SkillNode(){skillId = 1, skillNode = new TestSkillSequenceNode() });
        skillNodeDataList.AddToList(new SkillNode(){skillId = 1, skillNode = new TestSkillSequenceNode() });
    }
    
    private void SetMonsterSkillDataList()
    {
        //todo. 기획테이블 데이터를 TableDataHandler를 사용해 가공하여 데이터리스트에 추가List<MonsterSkillModel> monsterSkillDatas = new List<MonsterSkillModel>()
        //임시 데이터
        monsterSkillDataList.AddToList(new MonsterSkillModel(1, "MonsterSkill1", 1,0,0,0, 1f,10f, "첫번째 스킬입니다."));
        monsterSkillDataList.AddToList(new MonsterSkillModel(2, "MonsterSkill2",2, 0,0,0, 1f,10f, "두번째 스킬입니다."));
        monsterSkillDataList.AddToList(new MonsterSkillModel(3, "MonsterSkill3",2, 0,0,0, 1f,10f, "세번째 스킬입니다."));
        monsterSkillDataList.AddToList(new MonsterSkillModel(4, "MonsterSkill4",2, 0,0,0, 1f,10f,  "네번째 스킬입니다."));
        monsterSkillDataList.AddToList(new MonsterSkillModel(5, "MonsterSkill5",2, 0,0,0, 1f,10f,  "다섯번째 스킬입니다."));
    }
}
