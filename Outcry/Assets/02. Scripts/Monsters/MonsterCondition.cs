using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCondition : MonoBehaviour, IDamagable
{
    private MonsterBase monster;
     
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; } = false;
    
    public Action OnHealthChanged;
    public Action OnDeath;  //todo. think. BT 중지도 여기에 하면 될듯? 그럼 isDead 필요 없음? 고민해봐야할듯.

    private Coroutine animationCoroutine;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    private float stunAnimationLength;
    private float hitAnimationLength;   //일단 stun 애니메이션 길이로 맞춤.
    
    private void Start()
    {
        monster = GetComponent<MonsterBase>();
        if (monster == null)
        {
            Debug.LogError("MonsterCondition: MonsterBase component not found!");
            return;
        }

        Initialize();
        
        //stun 애니메이션 길이 가져오기
        RuntimeAnimatorController ac = monster.Animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == AnimatorStrings.MonsterAnimation.Stun)
            {
                stunAnimationLength = clip.length;
                //0.2로 나누어지는 수로 hitAnimationLength 설정.
                hitAnimationLength = Mathf.Floor(clip.length / 0.2f) * 0.2f;
                Debug.Log("Stun animation length: " + stunAnimationLength);
            }
        }
        
        //hit 애니메이션 관련
        spriteRenderer = monster.AttackController.GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void Initialize()    //오브젝트 풀이 필요할 것인가? 상정하고 짜뒀음.
    {
        SetMaxHealth();
    }

    private void SetMaxHealth()
    {
        MaxHealth = monster.MonsterData.health;
        CurrentHealth = MaxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        if (IsDead)
        {
            return;
        }
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        if (animationCoroutine != null)
        {
            spriteRenderer.color = originalColor;
            StopCoroutine(animationCoroutine);
        }

        if (CurrentHealth <= MaxHealth / 2)
        {
            monster.Animator.SetBool(AnimatorStrings.MonsterParameter.IsTired, true );
            if (CurrentHealth <= 0)
            {
                Death();
                return;
            }
        }
        else
        {
            animationCoroutine = StartCoroutine(HitAnimation(hitAnimationLength));
        }
    }
    
    //빨갛게 점멸하는 이펙트 코루틴
    private IEnumerator HitAnimation(float duration)
    {
        Color hitColor = Color.red;
        float flashDuration = 0.1f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            spriteRenderer.color = spriteRenderer.color == Color.red ? originalColor : hitColor;
            yield return new WaitForSeconds(flashDuration);
            elapsedTime += flashDuration;
        }

        spriteRenderer.color = originalColor;
    }

    public void Stunned()
    {
        monster.Animator.SetTrigger(AnimatorStrings.MonsterParameter.Stun);
        monster.MonsterAI.DisactivateBt();
        OnHealthChanged?.Invoke();
        
        StartCoroutine(WaitForBTActivation(stunAnimationLength));
    }
    
    private IEnumerator WaitForBTActivation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        monster.MonsterAI.ActivateBt();
    }
    
    private void Death()
    {
        CurrentHealth = 0;
        IsDead = true;
        monster.MonsterAI.DisactivateBt();
        monster.Animator.SetTrigger(AnimatorStrings.MonsterParameter.Dead);
    }
    
}
