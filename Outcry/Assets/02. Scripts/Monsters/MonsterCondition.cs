using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCondition : MonoBehaviour, IDamagable
{
    private MonsterBase monster;
     
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    public bool IsDead { get; private set; } = false;
    
    public Action OnHealthChanged;
    public Action OnDeath;  //todo. think. BT 중지도 여기에 하면 될듯? 그럼 isDead 필요 없음? 고민해봐야할듯.

    private float hitAnimationLength;
    
    private void Start()
    {
        monster = GetComponent<MonsterBase>();
        if (monster == null)
        {
            Debug.LogError("MonsterCondition: MonsterBase component not found!");
            return;
        }

        Initialize();
        
        RuntimeAnimatorController ac = monster.Animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == AnimatorStrings.MonsterAnimation.Hit)
            {
                hitAnimationLength = clip.length;
            }
        }
    }

    public void Initialize()    //오브젝트 풀이 필요할 것인가? 상정하고 짜뒀음.
    {
        SetMaxHealth();
    }

    private void SetMaxHealth()
    {
        maxHealth = monster.MonsterData.health;
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        Hit(damage);
        
        if (currentHealth <= 0)
        {
            Death();
        }

    }

    private void Hit(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        float hitTime = Time.time;
        monster.Animator.SetTrigger(AnimatorStrings.MonsterParameter.Hit);
        monster.MonsterAI.DisactivateBt();
        OnHealthChanged?.Invoke();
        
        StartCoroutine(WaitForBTActivation(hitAnimationLength));
    }
    private IEnumerator WaitForBTActivation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        monster.MonsterAI.ActivateBt();
    }
    private void Death()
    {
        currentHealth = 0;
        IsDead = true;
        monster.MonsterAI.DisactivateBt();
        monster.Animator.SetBool(AnimatorStrings.MonsterParameter.IsDead, true);
    }
    
}
