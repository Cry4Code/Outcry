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
    public SkillNodeDataList SkillNodeDataList => skillNodeDataList;
    // + dataList: DataListBase 
    // + monsterList: MonsterDataList
    // ------------ 
    // -Initialize(): void
    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        
        //SkillNode 리스트 초기화
        skillNodeDataList = new SkillNodeDataList();    
        skillNodeDataList.Initialize();
        SetSkillNodeDataList();
        
        //todo. 기획테이블 데이터를 TableDataHandler를 사용해 가공하여 데이터리스트에 추가
    }
    
    
    //스킬 노드 생성 코드 todo. think. 이게 베스트일까?
    private void SetSkillNodeDataList()
    {
        skillNodeDataList.AddToList(new SkillNode(){skillId = 1, skillNode = new TestSkillSequenceNode() });
        skillNodeDataList.AddToList(new SkillNode(){skillId = 1, skillNode = new TestSkillSequenceNode() });
    }
}
