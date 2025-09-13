using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class MonsterAIBase : MonoBehaviour
{
    protected MonsterBase monster;  //model은 이걸 타고 접근하는 걸로.
    
    [SerializeField]
    protected Node rootNode;
    protected Player target;
    protected bool isAttacking;

    protected void Awake()
    {
        monster = GetComponent<MonsterBase>();
        if (monster == null)
        {
            Debug.LogError("MonsterAI: MonsterBase component not found!");
            return;
        }
    }
    public abstract void InitializeBehaviorTree();  //외부에서 얘 호출되어야함.
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
