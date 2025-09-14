using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected MonsterModelBase monsterData;
    
    [Header("Components")]
    protected MonsterCondition condition;
    protected MonsterAIBase monsterAI;
    
    public MonsterModelBase MonsterData => monsterData;
    public MonsterCondition Condition => condition;
    public MonsterAIBase MonsterAI => monsterAI;
    
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

    protected void Update()
    {
        if (!condition.IsDead)
        {
            monsterAI.UpdateAI();
        }
    }
    public void SetMonsterData(MonsterModelBase monsterModel)
    {
        this.monsterData = monsterModel;
        if (monsterData != null)
            Initialize();
    }

    public void Initialize()
    {
        Debug.Log($"Initialize: {monsterData.monsterId}");
        monsterAI.Initialize(this);
        InitializeSkills();
    }
    protected abstract void InitializeSkills();
}
