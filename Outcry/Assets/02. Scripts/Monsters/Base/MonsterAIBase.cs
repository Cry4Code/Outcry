using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class MonsterAIBase : MonoBehaviour //MonoBehaviour 상속 안받아도 되는거 아닌감...? 근데 일단 인스펙터에서 확인해야하므로 상속 받게 함.
{
    protected MonsterBase monster;  //model은 이걸 타고 접근하는 걸로.
    
    
    protected Node rootNode;
    [SerializeField] protected Player target;
    public bool IsAttacking { get; private set; }

    // protected void Awake()
    // {
    //     monster = GetComponent<MonsterBase>();
    //     if (monster == null)
    //     {
    //         Debug.LogError("MonsterAI: MonsterBase component not found!");
    //         return;
    //     }
    // }
    public void Initialize(MonsterBase monster) //외부에서 얘 호출되어야함.
    {
        this.monster = monster;
        if (monster == null)
        {
            Debug.LogError("MonsterAI: MonsterBase component not found!");
            return;
        }
        InitializeBehaviorTree();
    }
    protected abstract void InitializeBehaviorTree(); 
    public void UpdateAI()
    {
        if (rootNode == null)
        {
            Debug.LogWarning("Root node is not assigned.");
            return;
        }

        NodeState state = rootNode.Tick();
    }
}
