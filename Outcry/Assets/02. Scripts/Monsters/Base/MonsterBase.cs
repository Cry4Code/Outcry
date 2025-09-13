using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour
{
    
    [Header("Components")]
    protected MonsterCondition condition;
    protected MonsterAIBase monsterAI;

    [Header("Data")]
    [SerializeField] protected MonsterModelBase monsterData;
    public MonsterModelBase MonsterData => monsterData;

    protected void Awake()
    {
        if (monsterData == null)
        {
            Debug.LogError("MonsterData is null");
        }
        condition = GetComponent<MonsterCondition>();
        if (condition == null)
        {
            condition = this.gameObject.AddComponent<MonsterCondition>();
        }
        monsterAI = GetComponent<MonsterAIBase>();
        if (monsterAI == null)
        {
            Debug.LogError(this.monsterData.monsterId + ": monsterAI is missing");
        }
        
        Initialize();
    }

    public void SetMonsterData(MonsterModelBase monsterModel)
    {
        this.monsterData = monsterModel;
        if (monsterData != null)
            Initialize();
    }
    public abstract void Initialize();
    protected abstract void InitializeSkills();
}
