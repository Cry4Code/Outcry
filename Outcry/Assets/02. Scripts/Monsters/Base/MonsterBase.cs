using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class MonsterBase : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected MonsterModelBase monsterData;
    
    [Header("Components")]
    protected MonsterCondition condition;
    protected MonsterAIBase monsterAI;
    protected Animator animator;
    protected MonsterAttackController attackController;
    protected Rigidbody2D rb2D;
    protected SpriteRenderer spriteRenderer;

    [Header("Hitbox")]
    [SerializeField] protected BoxCollider2D hitbox;
    
    public MonsterModelBase MonsterData => monsterData;
    public MonsterCondition Condition => condition;
    public MonsterAIBase MonsterAI => monsterAI;
    public Animator Animator => animator;
    public MonsterAttackController AttackController => attackController;
    public BoxCollider2D Hitbox => hitbox;  //attackController가 있으면.. hitBox도 갖고 있을 필요가 없을텐데...?
    public Rigidbody2D Rb2D => rb2D;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    
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

        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            Debug.LogError(this.monsterData.monsterId + ": rb2D is missing");
        }
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError(this.monsterData.monsterId + ": spriteRenderer is missing");
        }
        
        Debug.Log($"{monsterData.monsterId}가 Awake 되었습니다.");
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
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        float sign = transform.localScale.x >= 0 ? 1f : -1f;
        float offsetSizeX = transform.localScale.x;
        float offsetSizeY = transform.localScale.y;

        // 공격 범위 표시
        Gizmos.color = Color.red;
        if (hitbox != null && hitbox.enabled)
        {
            Vector3 offset = hitbox.offset;
            offset.x *= offsetSizeX;
            offset.y *= offsetSizeY;
            Vector3 scaledSize = new Vector3(hitbox.size.x * Mathf.Abs(offsetSizeX) * sign, hitbox.size.y * Mathf.Abs(offsetSizeY), 1f);
            Gizmos.DrawWireCube(transform.position + offset, scaledSize);
        }

        // 피격 범위 표시
        Gizmos.color = Color.yellow;
        BoxCollider2D hurtBox = GetComponent<BoxCollider2D>();
        if (hurtBox != null)
        {
            Vector3 offset = hurtBox.offset;
            offset.x *= Mathf.Abs(offsetSizeX);
            offset.y *= Mathf.Abs(offsetSizeY);
            Vector3 scaledSize = new Vector3(hurtBox.size.x * Mathf.Abs(offsetSizeX) * sign, hurtBox.size.y * Mathf.Abs(offsetSizeY), 1f);
            Gizmos.DrawWireCube(transform.position + offset, scaledSize);
        }

        // 접근 범위 표시
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, monsterData.detectRange * Mathf.Max(Mathf.Abs(offsetSizeX), Mathf.Abs(offsetSizeY)));
    }
#endif
}
