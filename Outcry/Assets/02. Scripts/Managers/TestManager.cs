using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    [SerializeField] private GameObject monsterPrefab;

    [SerializeField] private BossMonsterModel monsterData;
    
    private MonsterBase monster;
    
    void Awake()
    {
        DataManager.Instance.ToString();
        
        // BossMonsterModel monsterData = new BossMonsterModel(
        //     1, "BossMonster1", 100,
        //     10f, 3f, 10f, new int[0], new int[6] {103001, 103004, 103005, 103006, 103005, 103005});
        GameObject monsterObj = GameObject.Instantiate(monsterPrefab);
        
        monster = monsterObj.GetComponent<MonsterBase>();
        
        Vector3 scale = monster.transform.localScale;
        monster.transform.localScale = new Vector3(scale.x * 2f, scale.y * 2f, scale.z);
        
        monster.SetMonsterData(monsterData);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("TestManager: UpArrow key pressed. Monster takes 10 damage.");
            monster.Condition.TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("TestManager: DownArrow key pressed. Monster takes 10 damage.");
            monster.AttackController.CounterAttacked();
        }
    }
}
