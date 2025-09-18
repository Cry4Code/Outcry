using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeSkillSequenceNode : SkillSequenceNode
{
    // 경과 시간, 쿨타임 등 계산용
    [SerializeField] private float elapsedTime = 0f;

    // 스킬 작동 트리거
    private bool skillTriggered = false;

    // 애니메이션 클립 초당 프레임 수
    private const float ANIMATION_FRAME_RATE = 20f;
    // 투사체 생성 프레임
    private const float INSTANTIATE_STONE1_TIME = (1.0f / ANIMATION_FRAME_RATE) * 20;   // 20프레임이 지난 시점
    private const float INSTANTIATE_STONE2_TIME = (1.0f / ANIMATION_FRAME_RATE) * 28;   // 28프레임이 지난 시점

    // 투사체 오브젝트
    private GameObject stone;

    // 투사체 좌표?
    private float x1 = 0f;
    private float y1 = 0f;

    private float x2 = 0f;
    private float y2 = 0f;

    public EarthquakeSkillSequenceNode(int skillId) : base(skillId)
    {

    }

    public override void InitializeSkillSequenceNode(MonsterBase monster, Player target)
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
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= skillData.cooldown)
        {
            isCooldownComplete = true;
            elapsedTime = 0f;
        }
        else
        {
            isCooldownComplete = false;
        }

        result = isInRange && isCooldownComplete;
        Debug.Log($"Skill used? {result} : {elapsedTime} / {skillData.cooldown}");
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
            monster.Animator.SetTrigger(AnimatorStrings.MonsterParameter.Earthquake);

            // 플레이어 데미지 주기
            monster.AttackController.SetDamages(skillData.damage1);  

            skillTriggered = true;
        }

        // 시작 직후 Running 강제
        elapsedTime += Time.deltaTime;
        if (elapsedTime < 0.1f) // 시작 직후는 무조건 Running
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

            monster.AttackController.ResetDamages();  //데미지 초기화
            skillTriggered = false;
            state = NodeState.Success;
        }

        // 애니메이션의 동작 시간에 투사체(Stone) 생성 로직 실행
        if (elapsedTime == INSTANTIATE_STONE1_TIME)
        {
            //todo. 돌 프리팹 생성
            // 위치를 어떻게 잡?지?
            Object.Instantiate(stone, new Vector3(x1, y1, 0), Quaternion.identity);
        }                

        if (elapsedTime == INSTANTIATE_STONE2_TIME)
        {
            Object.Instantiate(stone, new Vector3(x2, y2, 0), Quaternion.identity);
        }

        return state;
    }
}
