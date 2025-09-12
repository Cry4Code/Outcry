using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour
{
    [Header("Data")]
    protected MonsterDataBase monsterData;
    
    [Header("Components")]
    protected MonsterCondition condition;
    protected MonsterAI monsterAI;
    protected Animator animator;

    [HideInInspector]
    public MonsterDataBase MonsterData => monsterData;
    public abstract void Initialize(MonsterDataBase monsterData);

    protected abstract void InitializeSkills();
}
