using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private GameObject monsterPrefab;
    
    void Start()
    {
        // BossMonsterModel monsterData = new BossMonsterModel(
        //         1, "BossMonster1", 100,
        //         10f, 3f, 10f, new int[2] {1,2}, new int[2] {2,2});
        // GameObject monsterObj = GameObject.Instantiate(monsterPrefab);
        //
        // MonsterBase monster = monsterObj.GetComponent<MonsterBase>();
        // monster.SetMonsterData(monsterData);


        TableDataHandler.LoadMonsterSkillData();
    }

}
