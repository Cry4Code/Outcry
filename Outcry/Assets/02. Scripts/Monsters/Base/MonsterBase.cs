using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour
{
    [Header("Data")]
    protected MonsterModelBase monsterModel;
    
    [Header("Components")]
    protected MonsterCondition condition;
    protected MonsterAI monsterAI;
    protected Animator animator;

    [HideInInspector]
    public MonsterModelBase MonsterModel => monsterModel;
    public abstract void Initialize(MonsterModelBase monsterModel);
    protected abstract void InitializeSkills();
}
