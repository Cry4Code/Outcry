using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class MonsterAIBase : MonoBehaviour //MonoBehaviour 상속 안받아도 되는거 아닌감...? 근데 일단 인스펙터에서 확인해야하므로 상속 받게 함.
{
    protected MonsterBase monster;  //model은 이걸 타고 접근하는 걸로.
    
    
    [SerializeField] protected SelectorNode rootNode;
    [SerializeField] protected Player target;

    private bool isAvailableToAct;
    public bool IsAttacking { get; protected set; }

    private float spawnAnimationLength;

    public virtual void Initialize(MonsterBase monster) //외부에서 호출되어야함.
    {
        target = PlayerManager.Instance.player;
        if (monster == null)
        {
            Debug.LogError("MonsterAI: MonsterBase component not found!");
            return;
        }
        this.monster = monster;
        InitializeBehaviorTree();
        IsAttacking = false;
        isAvailableToAct = false;
        //spawn 애니메이션 길이 가져오기
        RuntimeAnimatorController ac = monster.Animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == AnimatorStrings.MonsterAnimation.Spawn)
            {
                spawnAnimationLength = clip.length;
            }
        }
        StartCoroutine(ActivateMonster());
    }

    private IEnumerator ActivateMonster()
    {
        yield return new WaitForSeconds(spawnAnimationLength);
        isAvailableToAct = true;
    }
    protected abstract void InitializeBehaviorTree(); 
    
    public void UpdateAI()
    {
        if (!isAvailableToAct)
            return;
        if (rootNode == null)
        {
            Debug.LogWarning("Root node is not assigned.");
            return;
        }

        NodeState state = rootNode.Tick();
    }
    
    public void DisactivateBt()
    {
        isAvailableToAct = false;
    }

    public void ActivateBt()
    {
        isAvailableToAct = true;
    }
}
