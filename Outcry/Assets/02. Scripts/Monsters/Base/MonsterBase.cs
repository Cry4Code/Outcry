using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected MonsterModelBase monsterData;
    
    [Header("Components")]
    protected MonsterCondition condition;
    protected MonsterAIBase monsterAI;
    protected Animator animator;
    protected MonsterAttackController attackController;

    [Header("Hitbox")]
    [SerializeField] protected BoxCollider2D hitbox;
    
    public MonsterModelBase MonsterData => monsterData;
    public MonsterCondition Condition => condition;
    public MonsterAIBase MonsterAI => monsterAI;
    public Animator Animator => animator;
    public MonsterAttackController AttackController => attackController;
    public BoxCollider2D Hitbox => hitbox;  //attackController가 있으면.. hitBox도 갖고 있을 필요가 없을텐데...?
    
    protected void Awake()
    {
        if (monsterData == null)
        {
            Debug.LogError("MonsterData is null");
            return;
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
        
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError(this.monsterData.monsterId + ": animator is missing");
        }
        
        attackController = GetComponentInChildren<MonsterAttackController>();
        if (attackController == null)
        {
            animator.AddComponent<MonsterAttackController>();
        }
        
        if (hitbox == null)
        {
            Debug.LogError(this.monsterData.monsterId + ": hitBox is missing");
        }
        
        Initialize();
    }

    protected void FixedUpdate()
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
    
    void OnDrawGizmos() //히트박스 색상 변경
    {
        Gizmos.color = Color.red;

        if (hitbox != null && hitbox.enabled)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)hitbox.offset, hitbox.size);
        }
        
        Gizmos.color = Color.yellow;
        BoxCollider2D hurtBox = GetComponent<BoxCollider2D>();
        if (hurtBox != null)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)hurtBox.offset, hurtBox.size);
        }
    }
}
