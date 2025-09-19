using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAAI : MonsterAIBase
{
    private const float COMMON_SKILL_INTERVAL = 1f;

    // 트리 초기화
    protected override void InitializeBehaviorTree()
    {
        // 필요 노드들 생성
        // root 노드

        // SequenceNode - 전체 시퀀스
        // CanAttackConditionNode -
        // SelectorNode

        // 정찰 노드
        // PartrolActionNode 만들어서 사용

        // 몬스터 데이터를 일반 몬스터로 형변환
        // CommonMonsterModel monsterModel = (CommonMonsterModel)monster.MonsterData;

        // 일반 스킬 시퀀스 노드
        // SequenceNode
        // SelectorNode 그냥 Selector 노드로 생성 (셔플 X)
        // WaitActionNode (인터벌 1f)
        
        // SelectorNode (스킬 노드)
        // foreach로 2개 생성 monsterModel.commonSkillIds

        // 추적 노드
        // ChaseAcitionNode 사용 - 기존에 있음

        //this.rootNode = rootNode;
    }
}
