using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class MonsterAIBase : MonoBehaviour //MonoBehaviour 상속 안받아도 되는거 아닌감...? 근데 일단 인스펙터에서 확인해야하므로 상속 받게 함.
{
    protected MonsterBase monster;  //model은 이걸 타고 접근하는 걸로.
    
    
    [SerializeField] protected SelectorNode rootNode;
    [SerializeField] protected Player target;
    
    private bool isAvailableToAct = true;
    public bool IsAttacking { get; protected set; } //추후 Stun이나 다른 상태이상 추가.

    public void Initialize(MonsterBase monster) //외부에서 얘 호출되어야함.
    {
        if (monster == null)
        {
            Debug.LogError("MonsterAI: MonsterBase component not found!");
            return;
        }
        this.monster = monster;
        InitializeBehaviorTree();
        monster.Condition.OnDeath += DisactivateBt;   //죽으면 행동 못하게 막음.
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
    
    private void DisactivateBt()
    {
        isAvailableToAct = false;
    }

    private void ActivateBt()
    {
        isAvailableToAct = true;
    }
}
