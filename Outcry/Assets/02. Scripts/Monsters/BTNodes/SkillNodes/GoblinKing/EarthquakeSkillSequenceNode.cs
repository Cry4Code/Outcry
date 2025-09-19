using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EarthquakeSkillSequenceNode : SkillSequenceNode
{
    // 경과 시간, 쿨타임 등 계산용
    private float stateEnterTime; // 스킬(상태)에 진입한 시간

    // 애니메이션 클립 초당 프레임 수
    private const float ANIMATION_FRAME_RATE = 20f;
    // 투사체 생성 프레임
    private const float INSTANTIATE_STONE1_TIME = (1.0f / ANIMATION_FRAME_RATE) * 19;   // 20프레임이 지난 시점
    private const float INSTANTIATE_STONE2_TIME = (1.0f / ANIMATION_FRAME_RATE) * 25;   // 26프레임이 지난 시점
    private const float INSTANTIATE_STONE3_TIME = (1.0f / ANIMATION_FRAME_RATE) * 31;   // 32프레임이 지난 시점

    // 투사체 오브젝트
    private GameObject stone;

    // 투사체 좌표
    private Vector3 position1 = new Vector3(5.7f, -1.47f, 0f);
    private Vector3 position2 = new Vector3(10.2f, -1.47f, 0f);
    private Vector3 position3 = new Vector3(14.7f, -1.47f, 0f);

    // 투사체 생성 플레그
    private bool isSpawned1 = false;
    private bool isSpawned2 = false;
    private bool isSpawned3 = false;

    public EarthquakeSkillSequenceNode(int skillId) : base(skillId)
    {

    }

    public override void InitializeSkillSequenceNode(MonsterBase monster, PlayerController target)
    {
        base.InitializeSkillSequenceNode(monster, target);
        this.nodeName = "EarthquakeSkillSequenceNode";

        // 투사체 로드
        stone = ResourceManager.Instance.LoadAsset<GameObject>("Stone", Paths.Prefabs.Projectile);
    }

    protected override bool CanPerform()
    {
        bool result;
        bool isInRange;
        bool isCooldownComplete;

        // 플레이어와의 거리 4m 이내에 있을 때
        // todo. MonsterSkillModel에서 이걸 받아오도록 수정, 스킬 Stomp 참고
        if (Vector2.Distance(monster.transform.position, target.transform.position) <= skillData.range)
        {
            isInRange = true;
        }
        else
        {
            isInRange = false;
        }

        // 쿨다운 체크
        if (Time.time - lastUsedTime >= skillData.cooldown)
        {
            isCooldownComplete = true;
            isSpawned1 = false;
            isSpawned2 = false;
            isSpawned3 = false;
        }
        else
        {
            isCooldownComplete = false;
        }

        result = isInRange && isCooldownComplete;
        Debug.Log($"Skill {skillData.skillName} used? {result} : {Time.time - lastUsedTime} / {skillData.cooldown}");
        return result;
    }

    protected override NodeState SkillAction()
    {
        NodeState state;

        /*
        기본 피해 : HP 2칸 감소
        추가 효과 : 오브젝트(Stone) 생성 
                  - 각 오브젝트는 HP 1칸 감소

        **플레이어 대응**
            - 회피 사용 가능
            - 패링 사용 가능
        */

        // 스킬 트리거 켜기
        if (!skillTriggered)
        {
            lastUsedTime = Time.time;
            FlipCharacter();
            monster.Animator.SetTrigger(AnimatorStrings.MonsterParameter.Earthquake);
            monster.AttackController.SetDamages(skillData.damage1);  // 플레이어 데미지 주기

            skillTriggered = true;

            // 상태 시작 시간 저장
            stateEnterTime = Time.time;
        }

        // 시작 직후 Running 강제
        if (Time.time - lastUsedTime < 0.1f) // 시작 직후는 무조건 Running
        {
            return NodeState.Running;
        }

        // 애니메이션 중 Running 리턴 고정
        bool isSkillAnimationPlaying = IsSkillAnimationPlaying(AnimatorStrings.MonsterAnimation.Earthquake);
        if (isSkillAnimationPlaying)
        {
            Debug.Log($"Running skill: {skillData.skillName} (ID: {skillData.skillId})");
            state = NodeState.Running;
        }
        else
        {
            Debug.Log($"Running skill: {skillData.skillName} (ID: {skillData.skillId})");

            monster.AttackController.SetDamages(0);  //데미지 초기화
            skillTriggered = false;
            state = NodeState.Success;
        }

        // 애니메이션 경과 시간 계산
        float animationElapsedTime = Time.time - stateEnterTime;

        // 애니메이션의 동작 시간에 투사체(Stone) 생성 로직 실행
        if (animationElapsedTime >= INSTANTIATE_STONE1_TIME && !isSpawned1)
        {
            Debug.Log($"{skillData.skillName} : {stone.name} 생성 - 위치 {position1}");
            monster.AttackController.InstantiateProjectile(stone, position1);
            isSpawned1 = true;
        }      

        if (animationElapsedTime >= INSTANTIATE_STONE2_TIME && !isSpawned2)
        {
            Debug.Log($"{skillData.skillName} : {stone.name} 생성 - 위치 {position2}");
            monster.AttackController.InstantiateProjectile(stone, position2);
            isSpawned2 = true;
        }

        if (animationElapsedTime >= INSTANTIATE_STONE3_TIME && !isSpawned3)
        {
            Debug.Log($"{skillData.skillName} : {stone.name} 생성 - 위치 {position3}");
            monster.AttackController.InstantiateProjectile(stone, position3);
            isSpawned3 = true;
        }

        return state;
    }
}
